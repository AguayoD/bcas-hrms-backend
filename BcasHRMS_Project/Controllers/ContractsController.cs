﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Repositories.Service;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace BCAS_HRMSbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly tblContractsService _tblContractsService = new tblContractsService();

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetContractsByEmployeeId(int employeeId)
        {
            try
            {
                var allContracts = await _tblContractsService.GetAll();
                var employeeContracts = allContracts.Where(c => c.EmployeeID == employeeId).ToList();

                return Ok(employeeContracts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAlltblContracts()
        {
            try
            {
                var data = await _tblContractsService.GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdtblContracts(int id)
        {
            try
            {
                var data = await _tblContractsService.GetById(id);
                if (data == null) return NoContent();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InserttblContracts([FromForm] int employeeID, [FromForm] string contractType,
            [FromForm] string contractStartDate, [FromForm] string contractEndDate,
            [FromForm] int lastUpdatedBy, IFormFile file)
        {
            try
            {
                // Handle file upload
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Contracts");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create contract object
                var contract = new tblContracts
                {
                    EmployeeID = employeeID,
                    ContractType = contractType,
                    ContractStartDate = DateTime.Parse(contractStartDate),
                    ContractEndDate = DateTime.Parse(contractEndDate),
                    LastUpdatedBy = lastUpdatedBy,
                    LastUpdatedAt = DateTime.Now,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileType = file.ContentType,
                    FileSize = file.Length
                };

                var data = await _tblContractsService.Insert(contract);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContractFormData(int id,
            [FromForm] string? contractType,
            [FromForm] string? contractStartDate,
            [FromForm] string? contractEndDate,
            [FromForm] int lastUpdatedBy,
            IFormFile? file)
        {
            try
            {
                var existingContract = await _tblContractsService.GetById(id);
                if (existingContract == null) return NotFound();

                // Handle file upload if provided
                if (file != null && file.Length > 0)
                {
                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(existingContract.FilePath) && System.IO.File.Exists(existingContract.FilePath))
                    {
                        System.IO.File.Delete(existingContract.FilePath);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Contracts");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    existingContract.FileName = file.FileName;
                    existingContract.FilePath = filePath;
                    existingContract.FileType = file.ContentType;
                    existingContract.FileSize = file.Length;
                }

                // Update other fields if provided
                if (!string.IsNullOrEmpty(contractType))
                    existingContract.ContractType = contractType;

                if (!string.IsNullOrEmpty(contractStartDate))
                    existingContract.ContractStartDate = DateTime.Parse(contractStartDate);

                if (!string.IsNullOrEmpty(contractEndDate))
                    existingContract.ContractEndDate = DateTime.Parse(contractEndDate);

                existingContract.LastUpdatedBy = lastUpdatedBy;
                existingContract.LastUpdatedAt = DateTime.Now;

                var updatedData = await _tblContractsService.Update(existingContract);
                return Ok(updatedData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatetblContracts(int id, [FromBody] tblContracts tblContracts)
        {
            try
            {
                if (id != tblContracts.ContractID) return BadRequest("Id mismatched.");

                var data = await _tblContractsService.GetById(id);
                if (data == null) return NotFound();

                var updatedData = await _tblContractsService.Update(tblContracts);
                return Ok(updatedData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdtblContracts(int id)
        {
            try
            {
                Console.WriteLine($"Delete request received for contract ID: {id}");

                var data = await _tblContractsService.GetById(id);
                if (data == null)
                {
                    Console.WriteLine($"Contract with ID {id} not found");
                    return NotFound();
                }

                // Delete the associated file
                if (!string.IsNullOrEmpty(data.FilePath) && System.IO.File.Exists(data.FilePath))
                {
                    try
                    {
                        System.IO.File.Delete(data.FilePath);
                        Console.WriteLine($"File deleted: {data.FilePath}");
                    }
                    catch (Exception fileEx)
                    {
                        Console.WriteLine($"Warning: Failed to delete file {data.FilePath}: {fileEx.Message}");
                        // Continue with database deletion even if file deletion fails
                    }
                }

                var deletedData = await _tblContractsService.DeleteById(id);
                Console.WriteLine($"Contract deleted from database: {id}");
                return Ok(deletedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting contract {id}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadContract(int id)
        {
            try
            {
                var contract = await _tblContractsService.GetById(id);
                if (contract == null || string.IsNullOrEmpty(contract.FilePath))
                    return NotFound();

                if (!System.IO.File.Exists(contract.FilePath))
                    return NotFound("File not found");

                var memory = new MemoryStream();
                using (var stream = new FileStream(contract.FilePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory); // FIXED: Copy stream to memory instead of to itself
                }
                memory.Position = 0;

                var contentType = GetContentType(contract.FilePath);
                var fileName = contract.FileName ?? Path.GetFileName(contract.FilePath);

                return File(memory, contentType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading contract {id}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
            };
        }
    }
}