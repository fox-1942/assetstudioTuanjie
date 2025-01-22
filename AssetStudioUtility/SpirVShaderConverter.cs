using Smolv;
using SpirV;
using System;
using System.IO;
using System.Text;

namespace AssetStudio
{
    public static class SpirVShaderConverter
    {
        private static string outputDirectory;

        static SpirVShaderConverter()
        {
            outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ExPORTEDShaders");
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);

            }
            Directory.CreateDirectory(outputDirectory);
        }

        public static string Convert(byte[] m_ProgramCode)
        {
            var sb = new StringBuilder();
            using (var ms = new MemoryStream(m_ProgramCode))
            {
                using (var reader = new BinaryReader(ms))
                {
                    int requirements = reader.ReadInt32();
                    int minOffset = m_ProgramCode.Length;
                    int snippetCount = 5;
                    /*if (version[0] > 2019 || (version[0] == 2019 && version[1] >= 3)) //2019.3 and up
                    {
                        snippetCount = 6;
                    }*/
                    for (int i = 0; i < snippetCount; i++)
                    {
                        if (reader.BaseStream.Position >= minOffset)
                        {
                            break;
                        }

                        int offset = reader.ReadInt32();
                        int size = reader.ReadInt32();
                        if (size > 0)
                        {
                            if (offset < minOffset)
                            {
                                minOffset = offset;
                            }
                            var pos = ms.Position;
                            sb.Append(ExportSnippet(ms, offset, size));
                            ms.Position = pos;
                        }
                    }
                }
            }
            return sb.ToString();
        }

        private static string ExportSnippet(Stream stream, int offset, int size)
        {
            stream.Position = offset;
            int decodedSize = SmolvDecoder.GetDecodedBufferSize(stream);
            if (decodedSize == 0)
            {
                throw new Exception("Invalid SMOL-V shader header");
            }


            Guid fileid = Guid.NewGuid();
            SaveStreamToFile(stream, size, Path.Combine(outputDirectory, $"decoded_stream_{fileid}.smolv"));

          
            using (var decodedStream = new MemoryStream(new byte[decodedSize]))
            {
                if (SmolvDecoder.Decode(stream, size, decodedStream))
                {
                    SaveStreamToFile(decodedStream, decodedSize, Path.Combine(outputDirectory, $"decoded_stream_{fileid}.spv"));

                    decodedStream.Position = 0;
                    var module = Module.ReadFrom(decodedStream);
                    var disassembler = new Disassembler();
                  
                    return disassembler.Disassemble(module, DisassemblyOptions.Default).Replace("\r\n", "\n");
                }
                else
                {
                    throw new Exception("Unable to decode SMOL-V shader");
                }
            }


        }

        private static void SaveStreamToFile(Stream stream, int size, string fileName)
        {
            long originalPosition = stream.Position; 
            stream.Position = 0; // 

            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[stream.Length]; 
                stream.Read(buffer, 0, (int)stream.Length);
                fileStream.Write(buffer, 0, buffer.Length); 
            }

            stream.Position = originalPosition;
        }
    }
}
