using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoService.Domain.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream stream, string fileName, string contentType);
    }
}
