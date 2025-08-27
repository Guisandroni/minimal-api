
using Microsoft.EntityFrameworkCore.Query;
using minimal_api.Domain.Entity;
using minimal_api.Domain.DTO;
using minimal_api.Interfaces;
using minimal_api.Resources;

namespace minimal_api.Domain.Service;

public class AdminService : AdminInterfaceService
{

    private readonly DbRepository _dbRepository;

    public AdminService(DbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }
    public AdminEntity? Login(LoginDTO loginDto)
    {
        return _dbRepository.Admins.Where(a => a.Email == loginDto.Email && a.Password == loginDto.Password).FirstOrDefault();



    }



    public AdminEntity? GetAdminById(int id)
    {
        return _dbRepository.Admins.Where(a => a.Id == id).FirstOrDefault();
    }

    public AdminEntity? AddAdmin(AdminEntity Admins)
    {
        _dbRepository.Admins.Add(Admins);
        _dbRepository.SaveChanges();

        return Admins;
    }
    
      public List<AdminEntity> GetAllAdmins(int? pagina)
    {
        var query = _dbRepository.Admins.AsQueryable();

        int itensPorPagina = 10;

        if(pagina != null)
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }
}
