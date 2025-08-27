
using Microsoft.EntityFrameworkCore.Query;
using minimal_api.Domain.Entity;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Interface;
using minimal_api.Resources;

namespace minimal_api.Domain.Service;

public class AdminService : AdminInterfaceService
{

    private readonly DbRepository _dbRepository;

    public AdminService(DbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }
    public AdminEntity Login(LoginDTO loginDto)
    {
        return _dbRepository.Admins.Where(a => a.Email == loginDto.Email && a.Password == loginDto.Password).FirstOrDefault() 
               ?? throw new Exception("Admin not found");
    }

    public AdminEntity? GetAdminById(int id)
    {
        return _dbRepository.Admins.Where(a => a.Id == id).FirstOrDefault();
    }

    public AdminEntity? AddAdmin(AdminDTO adminDto)
    {
        var adminEntity = new AdminEntity
        {
            Email = adminDto.Email,
            Password = adminDto.Password,
            Perfil = adminDto.Perfil?.ToString() ?? "Editor"
        };
        
        _dbRepository.Admins.Add(adminEntity);
        _dbRepository.SaveChanges();

        return adminEntity;
    }
    
    public List<AdminEntity> GetAllAdmins()
    {
        return _dbRepository.Admins.ToList();
    }
}
