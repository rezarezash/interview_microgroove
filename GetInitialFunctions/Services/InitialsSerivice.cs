using GetInitialFunctions.Dtos;
using GetInitialFunctions.Entitites;
using Interv.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace GetInitialFunctions.Services
{
    public interface IInitialsSerivice
    {
        Task<int> InsertInitialsAsync(InitialsDto initialsDto, CancellationToken cancellationToken);
        Task<bool> UpdateInitialSvgAsync(int id, string svg, CancellationToken cancellationToken);
        Task<Initials?> GetInitialsAsync(int id, CancellationToken cancellationToken);
    }


    /// <summary>
    /// Provides services for managing <see cref="Initials"/> entities in the database.
    /// </summary>
    public sealed class InitialsSerivice : IInitialsSerivice
    {
        private readonly InitialsContext _initialsContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialsSerivice"/> class with the specified <see cref="InitialsContext"/>.
        /// </summary>
        /// <param name="initialsContext">The database context for <see cref="Initials"/> entities.</param>
        public InitialsSerivice(InitialsContext initialsContext) => _initialsContext = initialsContext;


        public async Task<Initials?> GetInitialsAsync(int id, CancellationToken cancellationToken)
        {
            return await _initialsContext.Initials.FindAsync(id, cancellationToken);
        }

        /// <summary>
        /// Inserts a new <see cref="Initials"/> entity into the database using the provided <see cref="InitialsDto"/>.
        /// </summary>
        /// <param name="initialsDto">The data transfer object containing the first and last name to insert.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// The id of the entity written to the database, or <c>null</c> if the operation fails.
        /// </returns>
        public async Task<int> InsertInitialsAsync(InitialsDto initialsDto,
                CancellationToken cancellationToken)
        {
            var newInitials = Initials.Create(initialsDto);

            await _initialsContext.Initials.AddAsync(newInitials, cancellationToken);

            var insertedCount = await _initialsContext.SaveChangesAsync(cancellationToken);

            return insertedCount > 0 ? newInitials.Id : 0;
        }

        /// <summary>
        /// Updates the SVG property of an existing <see cref="Initials"/> entity identified by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="Initials"/> entity to update.</param>
        /// <param name="svg">The SVG string to assign to the entity.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// The number of state entries written to the database, or <c>null</c> if the entity is not found.
        /// </returns>
        public async Task<bool> UpdateInitialSvgAsync(int id, string svg, CancellationToken cancellationToken)
        {
            var initials = await _initialsContext.Initials.FindAsync(id, cancellationToken);
            if (initials == null)
            {
                return false;
            }

            initials.Svg = svg;
            _initialsContext.Entry(initials).State = EntityState.Modified;

            return await _initialsContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
