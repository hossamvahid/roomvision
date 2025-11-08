using System;
using MongoDB.Driver;
using roomvision.domain.Entities;
using roomvision.domain.Interfaces.Mappers;
using roomvision.domain.Interfaces.Repositories;
using roomvision.infrastructure.Contexts;
using roomvision.infrastructure.Models;

namespace roomvision.infrastructure.Repositories;

public class FaceRepository : IFaceRepository
{
    private readonly MongoDbContext _context;
    private readonly IGenericMapper _mapper;

    public FaceRepository(MongoDbContext context, IGenericMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Face?> GetByIdAsync(string id)
    {
        var face = await _context.Faces.Find(face => face.Id == id).FirstOrDefaultAsync();
        return _mapper.Map<FaceDbModel, Face>(face);
    }

    public async Task AddAsync(Face face)
    {
        var faceDbModel = _mapper.Map<Face, FaceDbModel>(face);
        await _context.Faces.InsertOneAsync(faceDbModel);
    }

    public async Task UpdateAsync(Face face)
    {
        var faceDbModel = _mapper.Map<Face, FaceDbModel>(face);
        await _context.Faces.ReplaceOneAsync(f => f.Id == faceDbModel.Id, faceDbModel);
    }
    
    public async Task DeleteAsync(Face face)
    {
        var faceDbModel = _mapper.Map<Face, FaceDbModel>(face);
        await _context.Faces.DeleteOneAsync(f => f.Id == faceDbModel.Id);
    }

}
