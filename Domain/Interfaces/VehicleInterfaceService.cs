using minimal_api.Domain.Entity;

namespace minimal_api.Domain.Interface;


public interface VehicleInterfaceService
{

    List<VehicleEntity> GetAllVehicles(int pagina = 1, string? nome = null, string? marca = null, int? ano = null);
    VehicleEntity? GetVehicleById(int id);

    void  AddVehicle(VehicleEntity vehicle);



    void UpdateVehicle(VehicleEntity vehicle);

    void DeleteVehicle(int id);
   
      

}