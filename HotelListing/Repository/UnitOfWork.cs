using System;
using System.Threading.Tasks;
using HotelListing.Data;
using HotelListing.IRepository;

namespace HotelListing.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IGenericRepository<Country> _countries;
        private IGenericRepository<Hotel> _hotels;
        
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context);
        public IGenericRepository<Hotel> Hotels=>_hotels ??= new GenericRepository<Hotel>(_context);
        
        public async Task save()
        {
            await _context.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            //like garbage collector,like when i finish ,free the memory
            //free all the db context connections that the context was using  
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}