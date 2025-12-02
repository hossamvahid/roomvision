using System;
using MongoDB.Driver;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly MongoDbContext _context;
    private readonly IGenericMapper _mapper;

    public PersonRepository(MongoDbContext context, IGenericMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Person?> GetByIdAsync(string id)
    {
        var person = await _context.Persons.Find(person => person.Id == id).FirstOrDefaultAsync();
        return _mapper.Map<PersonDbModel, Person>(person);
    }

    public async Task<Person?> GetByNameAsync(string name)
    {
        var person = await _context.Persons.Find(person => person.Name == name).FirstOrDefaultAsync();
        return _mapper.Map<PersonDbModel, Person>(person);
    }

    public async Task AddAsync(Person person)
    {
        var personDbModel = _mapper.Map<Person, PersonDbModel>(person);
        await _context.Persons.InsertOneAsync(personDbModel);
    }

    public async Task UpdateAsync(Person person)
    {
        var personDbModel = _mapper.Map<Person, PersonDbModel>(person);
        await _context.Persons.ReplaceOneAsync(p => p.Id == personDbModel.Id, personDbModel);
    }
    
    public async Task DeleteAsync(Person person)
    {
        var personDbModel = _mapper.Map<Person, PersonDbModel>(person);
        await _context.Persons.DeleteOneAsync(p => p.Id == personDbModel.Id);
    }

}
