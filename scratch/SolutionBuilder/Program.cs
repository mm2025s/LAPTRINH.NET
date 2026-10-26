using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Scratch helper: ghép nhiều file nguồn thành 1 file để csc.exe (.NET Framework, C# 5)
// biên dịch, tránh vấn đề quoting với hàng trăm đường dẫn full path trong PowerShell.
//   SolutionBuilder <rootDir> <fileList1> [fileList2 ...]
// Mỗi fileList chứa danh sách đường dẫn tương đối so với rootDir (1 dòng 1 file).
// Tất cả using ở đầu file được gom lên đầu, các using trùng bị lược bỏ.
static class Program
{
    static string ReadSmart(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return new UTF8Encoding(false).GetString(bytes, 3, bytes.Length - 3);
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
        return new UTF8Encoding(false).GetString(bytes);
    }

    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: SolutionBuilder <rootDir> <list1> [list2 ...]");
            return 2;
        }

        string root = args[0];
        var usings = new List<string>();
        var seenUsing = new HashSet<string>(StringComparer.Ordinal);
        var body = new StringBuilder();

        for (int i = 1; i < args.Length; i++)
        {
            string listPath = Path.IsPathRooted(args[i]) ? args[i] : Path.Combine(root, args[i]);
            if (!File.Exists(listPath)) { Console.Error.WriteLine("Missing list: " + listPath); return 3; }

            foreach (string raw in File.ReadAllLines(listPath))
            {
                string rel = raw.Trim();
                if (rel.Length == 0 || rel.StartsWith("#")) continue;

                string full = Path.Combine(root, rel);
                if (!File.Exists(full)) { Console.Error.WriteLine("Missing source: " + full); return 4; }

                string[] lines = ReadSmart(full).Replace("\r\n", "\n").Replace("\n", "\r\n").Split(new[] { "\r\n" }, StringSplitOptions.None);

                body.AppendLine();
                body.AppendLine("// ========== " + rel.Replace('\\', '/') + " ==========");
                foreach (string line in lines)
                {
                    string t = line.Trim();
                    if (t.StartsWith("using ") && t.EndsWith(";") && !t.StartsWith("using (") && !t.StartsWith("using(") && t.IndexOf(" = ") < 0 && t.IndexOf("=>") < 0)
                    {
                        if (seenUsing.Add(t)) usings.Add(t);
                        continue;
                    }
                    body.AppendLine(line);
                }
            }
        }

        usings.Sort(StringComparer.Ordinal);
        var sb = new StringBuilder();
        foreach (string u in usings) sb.AppendLine(u);
        sb.Append(body);
        Console.Out.Write(sb.ToString());
        return 0;
    }
}
