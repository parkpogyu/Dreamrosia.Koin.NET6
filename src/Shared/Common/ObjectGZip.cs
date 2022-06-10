using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Shared.Common
{
    public static class ObjectGZip
    {

        public static async Task<byte[]> CompressAsync<T>(this T obj)
        {
            using var dataStream = new MemoryStream();
            using var dataStreamWriter = new StreamWriter(dataStream);

            //Serialize object to MemoryStream
            await dataStreamWriter.WriteAsync(JsonSerializer.Serialize(obj));
            await dataStreamWriter.FlushAsync();

            //using var dataStream = new MemoryStream(JsonSerializer.Serialize(obj));

            //Ensure we reset to the begining of the stream to be able to read from it.
            dataStream.Position = 0;

            using var dataStreamCompressed = new MemoryStream();
            //Initialize the compression steam that will do the compression work.
            //Use the destination stream as a constructor parameter, and specify the desired compression level.
            using var dataStreamCompression = new GZipStream(dataStreamCompressed, CompressionMode.Compress, leaveOpen: true);

            //Copy the source data strean, to the compression stream. Once this operation is complete the `dataStreamCompressed` will contain the compressed version of the srouce stream
            await dataStream.CopyToAsync(dataStreamCompression);
            await dataStreamCompression.FlushAsync();

            var bytes = dataStreamCompressed.ToArray();

            //return dataStreamCompressed.ToArray();

            return bytes;
        }

        public static async Task<T> DecompressAsync<T>(this byte[] data)
        {
            MemoryStream dataStream = new MemoryStream(data);

            dataStream.Position = 0;

            using var dataStreamDecompressed = new MemoryStream();

            //Initialize the decompression steam that will do the decompression work.
            //Use the sourceStream as a constructor parameter and specify the Decompress mode.
            //Ensure that the source stream (dataStreamCompressed) position is 0.
            using var dataStreamDecompression = new GZipStream(dataStream, CompressionMode.Decompress, leaveOpen: true);

            //Copy the information from the source, decompressionStream to the destination stream, the decompressed stream
            await dataStreamDecompression.CopyToAsync(dataStreamDecompressed);
            await dataStreamDecompressed.FlushAsync();

            dataStreamDecompressed.Position = 0;

            using var dataStreamReader = new StreamReader(dataStreamDecompressed);

            var obj = JsonSerializer.Deserialize<T>(await dataStreamReader.ReadToEndAsync());

            return obj;

            //return JsonSerializer.Deserialize<T>(await dataStreamReader.ReadToEndAsync());
        }
    }
}
