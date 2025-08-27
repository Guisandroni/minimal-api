using minimal_api.Domain.Entity;
using minimal_api.Model.DTO;

namespace minimal_api.Domain.Interface;


public interface AdminInterfaceService
{

    AdminEntity Login(LoginDTO loginDto);

    AdminEntity? AddAdmin(AdminDTO adminDto);
    List<AdminEntity> GetAllAdmins();
    AdminEntity? GetAdminById(int id);
    

}