
using Microsoft.EntityFrameworkCore.Query;
using minimal_api.Domain.Entity;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Interface;
using minimal_api.Resources;

namespace minimal_api.Domain.Service;

public class VehicleService : VehicleInterfaceService
{

    private readonly DbRepository _dbRepository;

    public VehicleService(DbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }

    public void DeleteVehicle(int id)
    {
        var vehicle = _dbRepository.Vehicles.Find(id);
        if (vehicle != null)
        {
            _dbRepository.Vehicles.Remove(vehicle);
            _dbRepository.SaveChanges();
        }
    }

    public void UpdateVehicle(VehicleEntity vehicle)
    {
        _dbRepository.Vehicles.Update(vehicle);
        _dbRepository.SaveChanges();
    }

    public VehicleEntity? GetVehicleById(int id)
    {
        return _dbRepository.Vehicles.Find(id);
    }

    public void AddVehicle(VehicleEntity vehicle)
    {
        _dbRepository.Vehicles.Add(vehicle);
        _dbRepository.SaveChanges();
    }

    public List<VehicleEntity> GetAllVehicles(int pagina = 1, string? nome = null, string? marca = null, int? ano = null)
    {
        int pageSize = 10;
        var query = _dbRepository.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => v.Nome.Contains(nome));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => v.Marca.Contains(marca));
        }

        if (ano.HasValue)
        {
            query = query.Where(v => v.Ano == ano.Value);
        }

        return query.Skip((pagina - 1) * pageSize).Take(pageSize).ToList();
    }
}
