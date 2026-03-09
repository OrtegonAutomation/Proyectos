const geminiResponse = $node["GeminiOCR"].json.text || "";
let parsedData = {};

try {
  const jsonMatch = geminiResponse.match(/\{[\s\S]*\}/);
  if (jsonMatch) {
    parsedData = JSON.parse(jsonMatch[0]);
  } else {
    parsedData = { codigo: 'No detectado', version: 'No detectado', vigencia: 'No detectado' };
  }
} catch (e) {
  parsedData = { codigo: 'Error', version: 'Error', vigencia: 'Error' };
}

return {
  Fecha: new Date().toISOString(),
  'Telegram File ID': $node["TelegramTrigger"].json["message"]["photo"]?.pop()?.["file_id"] || 'N/A',
  'Código': parsedData.codigo || 'N/A',
  'Versión': parsedData.version || 'N/A',
  'Vigencia': parsedData.vigencia || 'N/A',
  'Confianza': 1.0, 
  'Estado': (parsedData.codigo && parsedData.codigo !== 'N/A' && parsedData.codigo !== 'No detectado') ? 'Procesado' : 'Requiere Revisión',
  'Texto Completo': geminiResponse
};