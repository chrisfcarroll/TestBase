﻿using System.Text;

namespace TestBase
{
    public static class Pdf
    {
        /// <summary> Because PDF standard is ASCII</summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static byte[] AsciiBytes(string line="This is a small text editable pdf") => Encoding.ASCII.GetBytes(DocumentWithLineOfText(line));

        public static string DocumentWithLineOfText(string line="This is a small text editable pdf") => $@"%PDF-1.4
1 0 obj
<< /Type /Catalog
/Outlines 2 0 R
/Pages 3 0 R
>>
endobj
2 0 obj
<< /Type /Outlines
/Count 0
>>
endobj
3 0 obj
<< /Type /Pages
/Kids [4 0 R]
/Count 1
>>
endobj
4 0 obj
<< /Type /Page
/Parent 3 0 R
/MediaBox [0 0 612 144]
/Contents 5 0 R
/Resources << /ProcSet 6 0 R
/Font << /F1 7 0 R >>
>>
>>
endobj
5 0 obj
<< /Length 73 >>
stream
BT
/F1 24 Tf
100 100 Td
({line}) Tj
ET
endstream
endobj
6 0 obj
[/PDF /Text]
endobj
7 0 obj
<< /Type /Font
/Subtype /Type1
/Name /F1
/BaseFont /Helvetica
/Encoding /MacRomanEncoding
>>
endobj
xref
0 8
0000000000 65535 f
0000000009 00000 n
0000000074 00000 n
0000000120 00000 n
0000000179 00000 n
0000000364 00000 n
0000000466 00000 n
0000000496 00000 n
trailer
<< /Size 8
/Root 1 0 R
>>
startxref
625
%%EOF";
    }
}
