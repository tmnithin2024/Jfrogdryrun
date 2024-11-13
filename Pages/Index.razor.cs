using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFileUpload_Net7.Pages
{
    public partial class Index
    {
        IReadOnlyList<IBrowserFile> files;
        List<string> urls = new List<string>();
        string dropClass = string.Empty;
        const int maxFileSize = 512000;
        [Inject] IWebHostEnvironment _env { get; set; }

        private void HandleDragEnter()
        {
            dropClass = "dropdivOn";
        }

        private void HandleDragLeave()
        {
            dropClass = string.Empty;
        }

        async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            dropClass = string.Empty;
            try
            {
                
                    files = null;

                    var url = await SaveFile(e.File);
                    urls.Clear();
                    urls.Add(url);
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }
        

        private async Task<string> SaveFile(IBrowserFile file, string guid = null)
        {
            if (guid == null)
            {
                guid = Guid.NewGuid().ToString();
            }

            var relativePath = Path.Combine("uploads", guid);
            var dirToSave = Path.Combine(_env.WebRootPath, relativePath);
            var di = new DirectoryInfo(dirToSave);
            if (!di.Exists)
            {
                di.Create();
            }

            var filePath = Path.Combine(dirToSave, file.Name);
            using (var stream = file.OpenReadStream(maxFileSize))
            {
                using (var mstream = new MemoryStream())
                {
                    await stream.CopyToAsync(mstream);
                    await File.WriteAllBytesAsync(filePath, mstream.ToArray());
                }
            }

            var url = Path.Combine(relativePath, file.Name).Replace("\\", "/");
            return url;
        }
    }
}
