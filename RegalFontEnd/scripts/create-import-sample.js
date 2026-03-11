const fs = require('fs');
const path = require('path');
const XLSX = require('xlsx');
const dir = path.join(__dirname, '..', 'sample-data');
if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
const data = [
  {
    'Student Code': 'SAMPLE001',
    'Student name': 'Nguy?n Minh Anh',
    'Advisor code': 'ADV001',
    Gender: 'Female',
    'Date of Birth': '2008-04-15',
    Phone: '0912345678',
    Email: 'minhanh@example.com',
    School: 'THCS Lê L?i',
    Source: 'Fanpage',
    Job: 'H?c sinh'
  },
  {
    'Student Code': 'SAMPLE002',
    'Student name': 'Tr?n Qu?c Khánh',
    'Advisor code': 'ADV002',
    Gender: 'Male',
    'Date of Birth': '2007-11-02',
    Phone: '0987654321',
    Email: 'quockhanh@example.com',
    School: 'THPT Nguy?n Trãi',
    Source: 'Gi?i thi?u t? b?n',
    Job: 'H?c sinh'
  }
];
const ws = XLSX.utils.json_to_sheet(data);
const wb = XLSX.utils.book_new();
XLSX.utils.book_append_sheet(wb, ws, 'Participants');
const filePath = path.join(dir, 'company-event-participants-import.xlsx');
XLSX.writeFile(wb, filePath);
console.log('created', filePath);
