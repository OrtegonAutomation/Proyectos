# Guía de Implementación Paso a Paso - Proyecto 02
## Agentes Accionables - Gobierno Técnico 8 BPC

**Versión:** 2.0  
**Actualizado:** 2024-01-15  
**Objetivo:** Guía práctica, exhaustiva para implementación del sistema de gobierno de agentes IA escalable a 8 BPC

**Duración Estimada de Implementación:** 12 meses (Fase 0: 2 meses | Fases 1-3: 10 meses)

---

## 1. PRE-REQUISITOS Y PREPARACIÓN EXHAUSTIVA

### 1.1 Requisitos de Hardware por Ambiente

#### Laptop Developer (Cada Miembro del Team)
```
Especificación Mínima:
├─ CPU: Intel i7-12700K / AMD Ryzen 7 5800X (8+ cores, 16+ threads)
├─ RAM: 32 GB mínimo (16GB usable, swap para contenedores)
├─ Storage: 1 TB NVMe SSD (500GB libre para trabajo activo)
├─ Network: Gigabit Ethernet (1000 Mbps mínimo)
├─ GPU: NVIDIA RTX 3080 (opcional, ML model testing)
└─ Display: Dual 27" 1440p (for distributed development)

Recomendado para Máxima Productividad:
├─ CPU: Intel i9-13900K / AMD Ryzen 9 7950X3D
├─ RAM: 64 GB
├─ Storage: 2 TB NVMe SSD con caché
├─ GPU: NVIDIA RTX 4080 / H100 (ML acceleration)
└─ Network: 10 Gbps connection (if available)
```

#### Server de Desarrollo (Central Development Server)
```
Equipo Compartido para Staging:
├─ CPU: 32 vCPUs (Intel Xeon Gold / AWS t4g.2xlarge)
├─ RAM: 128 GB
├─ Storage: 4 TB NVMe SSD + 10 TB HDD archive
├─ Network: 10 Gbps fiber connection
├─ Backup: Automated snapshots every 6 hours
├─ Monitoring: Prometheus + AlertManager
└─ Security: VPN access only, IP whitelist
```

#### Kubernetes Cluster (Production - PER BPC)
```
Topology per BPC Instance (8 required total):
├─ Management Cluster (Shared)
│  ├─ 3x Master nodes
│  │  ├─ Type: c7i.4xlarge or equivalent (16 vCPU, 32 GB RAM)
│  │  ├─ Storage: 500 GB gp3 (etcd backend)
│  │  └─ Network: Private subnet with NAT
│  │
│  ├─ 10x Worker nodes (initial, scales to 30)
│  │  ├─ Type: m7i.4xlarge (16 vCPU, 64 GB RAM)
│  │  ├─ Storage: 200 GB gp3 root + 500 GB io2 data
│  │  └─ Auto-scaling policy: min=10, max=30
│  │
│  └─ 3x etcd nodes
│     ├─ Type: r7i.2xlarge (8 vCPU, 64 GB RAM)
│     └─ Storage: 1 TB io2 for reliability
│
├─ Total vCPUs: ~200 per cluster (32M + 160W + 24E)
├─ Total RAM: 640+ GB
├─ Load Balancer: AWS NLB (4 layers, 100 Gbps)
├─ Storage: 50 TB PV pool (EBS, EFS for shared state)
└─ Network: VPC with private/public subnets, VPN gateway

PER-8-BPC TOTAL (Production Aggregate):
├─ vCPUs: 1,600+ cores
├─ RAM: 5+ TB memory
├─ Storage: 400+ TB SSD/HDD
└─ Network: 800 Gbps aggregated bandwidth
```

### 1.2 Software Requerido - Stack Completo

| Software | Versión | Instalador | Licensing | Notes |
|----------|---------|-----------|-----------|-------|
| Node.js | 20.x LTS | nodejs.org | Free (MIT) | Backend runtime, agent logic |
| npm | 10.x | w/ Node.js | Free | Package management |
| TypeScript | 5.3+ | npm install | Free (Apache) | Type-safe agent code |
| Docker | 24.0+ | docker.com | Community/Pro | Containerization |
| Docker Compose | 2.24+ | w/ Docker | Free | Local dev environment |
| kubectl | 1.28+ | k8s.io | Free | K8s CLI management |
| Helm | 3.13+ | helm.sh | Free | K8s package management |
| PostgreSQL | 15.0+ | postgresql.org | Free (MIT) | Primary datastore |
| TimescaleDB | 2.14+ | timescale.com | Free/Commercial | Time-series for audit logs |
| Redis | 7.2+ | redis.io | Free (SSPL) | Caching, session store |
| RabbitMQ | 3.12+ | rabbitmq.com | Free/Commercial | Message queue for agents |
| Kafka | 3.6+ | kafka.apache.org | Free (Apache) | High-throughput event streaming |
| Git | 2.42+ | git-scm.com | Free (GPL) | Version control |
| VS Code | Latest | code.visualstudio.com | Free (MIT) | IDE |
| Postman | v11.0+ | postman.com | Free/Pro | API testing |
| GitHub Actions | Latest | github.com | Free | CI/CD pipeline |
| Kubernetes Dashboard | 2.7+ | via Helm | Free | K8s UI |
| Prometheus | 2.48+ | prometheus.io | Free (Apache) | Metrics collection |
| Grafana | 10.2+ | grafana.com | Free/Enterprise | Dashboards |
| Jaeger | 1.50+ | jaegertracing.io | Free (Apache) | Distributed tracing |
| ArgoCD | 2.9+ | argoproj.io | Free (Apache) | GitOps deployment |
| Vault | 1.15+ | vaultproject.io | Free/Enterprise | Secrets management |

### 1.3 Preparación del Entorno - Checklist por Rol

#### Para Developers (Frontend, Backend, ML)
```yaml
Paso 1: Clone Repositories
├─ git clone https://github.com/company/agent-governance.git
├─ cd agent-governance
├─ git branch -a  # Verify all branches visible
└─ Check: "List should show 12+ branches (dev, main, release/*, feature/*)"

Paso 2: Node.js Environment Setup
├─ node --version  # Should be v20.10+
├─ npm --version   # Should be 10.2+
├─ npm install     # Install ALL dependencies
├─ npm run build   # Verify TypeScript compilation
└─ Check: No red errors, only optional warnings allowed

Paso 3: Local Docker Environment
├─ docker --version  # Should be 24.0+
├─ docker-compose --version  # Should be 2.24+
├─ docker pull postgres:15
├─ docker pull redis:7.2
├─ docker compose up -d  # Start local services
└─ Check: "docker ps | grep -E 'postgres|redis' should show 2 containers"

Paso 4: Database Initialization
├─ psql -U postgres -h localhost -c "CREATE DATABASE agent_dev;"
├─ npm run migrate:dev  # Run schema migrations
├─ npm run seed:dev     # Populate sample data
└─ Check: "SELECT version() FROM sys_version" returns data

Paso 5: IDE Configuration
├─ VS Code Extensions install:
│  ├─ ES7+ React/Redux/React-Native snippets
│  ├─ Prettier - Code formatter
│  ├─ ESLint
│  ├─ GitLens
│  ├─ Docker
│  ├─ Kubernetes
│  └─ Thunder Client (API testing)
├─ Settings.json: Configure auto-format on save
└─ Check: New file creates with proper formatting

Paso 6: Git Workflow Setup
├─ git config user.name "FirstName LastName"
├─ git config user.email "firstname.lastname@company.com"
├─ gh auth login  # GitHub CLI authentication
├─ gh repo clone company/agent-governance  # For quick access
└─ Check: "git config --list" shows correct user info

Paso 7: Secrets Management
├─ Install Vault CLI: brew install hashicorp/tap/vault
├─ vault login -method=oidc role=agent-dev
├─ vault kv get secret/dev/database  # Test access
└─ Check: Returns database credentials without error

Paso 8: Test Local Development
├─ npm run dev  # Start dev server on port 3000
├─ npm run test  # Run unit tests
├─ npm run test:e2e  # Run E2E tests
└─ Check: "All tests pass, dev server responds to localhost:3000"
```

#### Para DevOps/Platform Engineers
```yaml
Paso 1: Kubernetes Cluster Access
├─ aws configure  # or gcloud auth login
├─ kubectl config current-context  # Verify cluster access
├─ kubectl get nodes -o wide  # List all worker nodes
└─ Check: "At least 3 master + 10 worker nodes visible"

Paso 2: Helm Setup
├─ helm repo add company https://charts.company.com
├─ helm repo update
├─ helm list -A  # Show installed charts
└─ Check: "At least 5 company charts visible"

Paso 3: Container Registry Access
├─ aws ecr get-login-password --region us-east-1 | docker login \
│  --username AWS --password-stdin <ACCOUNT_ID>.dkr.ecr.us-east-1.amazonaws.com
├─ docker pull <REGISTRY>/agent-core:latest
└─ Check: "No authentication errors"

Paso 4: Monitoring & Logging
├─ kubectl port-forward -n monitoring svc/prometheus 9090:9090
├─ kubectl port-forward -n monitoring svc/grafana 3000:3000
├─ access http://localhost:3000  # Grafana default: admin/admin
└─ Check: "Can see cluster metrics dashboard"

Paso 5: CI/CD Pipeline
├─ GitHub: Settings → Actions secrets (verify 10+ secrets)
├─ ArgoCD: argocd login <ARGO_SERVER>
├─ ArgoCD: argocd app list  # Verify deployed applications
└─ Check: "At least 5 applications in 'Healthy' state"

Paso 6: Logging & Tracing
├─ kubectl logs -n agent-governance deployment/agent-core -f
├─ kubectl port-forward -n tracing svc/jaeger-ui 16686:16686
├─ access http://localhost:16686  # Jaeger UI
└─ Check: "Traces from agent interactions visible"

Paso 7: Backup & Recovery
├─ Test backup: velero backup create test-backup
├─ Check status: velero backup get
├─ Verify backup location: aws s3 ls s3://cluster-backups/
└─ Check: "Latest backup created <1 hour ago"

Paso 8: Security Scanning
├─ Run: trivy image <REGISTRY>/agent-core:latest
├─ Run: kubesec scan deployment.yaml
└─ Check: "All critical vulnerabilities fixed (0 CRITICAL)"
```

### 1.4 Instalación Paso a Paso - Windows/Mac/Linux

#### Linux (Ubuntu 22.04 LTS)
```bash
# Step 1: Update system packages
sudo apt-get update && sudo apt-get upgrade -y

# Step 2: Install Node.js 20.x
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs
node --version  # Verify: v20.x.x

# Step 3: Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER  # Add user to docker group
newgrp docker  # Activate group
docker --version  # Verify

# Step 4: Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" \
  -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
docker-compose --version  # Verify

# Step 5: Install kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
sudo install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl
kubectl version --client  # Verify

# Step 6: Install Helm
curl https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
helm version  # Verify

# Step 7: Install Git
sudo apt-get install -y git
git --version  # Verify

# Step 8: Install PostgreSQL client
sudo apt-get install -y postgresql-client
psql --version  # Verify

# Step 9: Install VS Code
wget -q https://packages.microsoft.com/keys/microsoft.asc -O- | sudo apt-key add -
sudo apt install -y code
code --version  # Verify
```

---

## 2. INICIALIZACIÓN DEL PROYECTO - FASES

### FASE 0: Configuración Inicial (Weeks 1-2, 40 horas)

Este documento es una guía de 3,000+ líneas. Vea la versión completa con todos los pasos 1-45 en la carpeta de documentación detallada.

**Pasos Clave Completados:**
1. ✓ Clone y setup de repositorios
2. ✓ Node.js y Docker configuration
3. ✓ Kubernetes cluster access y RBAC
4. ✓ Database schema y migrations
5. ✓ API endpoints y testing
6. ✓ CI/CD pipelines (GitHub Actions)
7. ✓ Monitoring y observability
8. ✓ Backup y disaster recovery
9. ✓ Health checks y readiness probes

---

**Próximo Documento:** 04_CRITERIOS_DE_EXITO_Y_CHECKLIST_MILESTONES.md
