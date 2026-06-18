// Autor:   DemoTareas Team
// Fecha:   2026-06-18
// Versión: 1.0

using DemoTareas.Models;
using DemoTareas.Repositories;

namespace DemoTareas.Services;

public class PersonaService : IPersonaService
{
    private readonly IPersonaRepository _repository;

    public PersonaService(IPersonaRepository repository)
    {
        _repository = repository;
    }

    public IList<Persona> GetAll() => _repository.GetAll();

    public void Add(Persona persona)
    {
        ValidarPersona(persona);

        if (_repository.GetByNombreCompleto(persona.Nombre, persona.Apellido) is not null)
            throw new InvalidOperationException($"Ya existe una persona con el nombre '{persona.Nombre} {persona.Apellido}'.");

        _repository.Add(persona);
    }

    public void Update(Persona persona)
    {
        ValidarPersona(persona);

        var existente = _repository.GetByNombreCompleto(persona.Nombre, persona.Apellido);
        if (existente is not null && existente.Id != persona.Id)
            throw new InvalidOperationException($"Ya existe una persona con el nombre '{persona.Nombre} {persona.Apellido}'.");

        var actual = _repository.GetById(persona.Id)
            ?? throw new InvalidOperationException($"No se encontró la persona con Id {persona.Id}.");

        actual.Nombre = persona.Nombre.Trim();
        actual.Apellido = persona.Apellido.Trim();
        actual.Edad = persona.Edad;
        actual.Departamento = persona.Departamento?.Trim();

        _repository.Update(actual);
    }

    public void Delete(int id) => _repository.Delete(id);

    private static void ValidarPersona(Persona persona)
    {
        if (string.IsNullOrWhiteSpace(persona.Nombre))
            throw new ArgumentException("El nombre de la persona no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(persona.Apellido))
            throw new ArgumentException("El apellido de la persona no puede estar vacío.");

        if (persona.Edad < 0 || persona.Edad > 120)
            throw new ArgumentException("La edad debe estar entre 0 y 120 años.");
    }
}
