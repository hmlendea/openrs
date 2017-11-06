using System.Collections.Generic;
using System.IO;
using System.Linq;

using RuneScapeSolo.DataAccess.DataObjects;
using RuneScapeSolo.DataAccess.Exceptions;

namespace RuneScapeSolo.DataAccess.Repositories
{
    /// <summary>
    /// gameModel repository implementation.
    /// </summary>
    public class GameModelRepository
    {
        readonly XmlDatabase<GameModelEntity> xmlDatabase;
        List<GameModelEntity> gameModelEntities;
        bool loadedEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModelRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public GameModelRepository(string fileName)
        {
            xmlDatabase = new XmlDatabase<GameModelEntity>(fileName);
            gameModelEntities = new List<GameModelEntity>();
        }

        public void ApplyChanges()
        {
            try
            {
                xmlDatabase.SaveEntities(gameModelEntities);
            }
            catch
            {
                // TODO: Better exception message
                throw new IOException("Cannot save the changes");
            }
        }

        /// <summary>
        /// Adds the specified game model.
        /// </summary>
        /// <param name="gameModelEntity">Game model.</param>
        public void Add(GameModelEntity gameModelEntity)
        {
            LoadEntitiesIfNeeded();

            gameModelEntities.Add(gameModelEntity);
        }

        /// <summary>
        /// Get the game model with the specified identifier.
        /// </summary>
        /// <returns>The game model.</returns>
        /// <param name="id">Identifier.</param>
        public GameModelEntity Get(string id)
        {
            LoadEntitiesIfNeeded();

            GameModelEntity gameModelEntity = gameModelEntities.FirstOrDefault(x => x.Id == id);

            if (gameModelEntity == null)
            {
                throw new EntityNotFoundException(id, nameof(GameModelEntity).Replace("Entity", ""));
            }

            return gameModelEntity;
        }

        /// <summary>
        /// Gets all the game models.
        /// </summary>
        /// <returns>The game models</returns>
        public IEnumerable<GameModelEntity> GetAll()
        {
            LoadEntitiesIfNeeded();

            return gameModelEntities;
        }

        /// <summary>
        /// Updates the specified game model.
        /// </summary>
        /// <param name="gameModelEntity">Game model.</param>
        public void Update(GameModelEntity gameModelEntity)
        {
            LoadEntitiesIfNeeded();

            GameModelEntity gameModelEntityToUpdate = gameModelEntities.FirstOrDefault(x => x.Id == gameModelEntity.Id);

            if (gameModelEntityToUpdate == null)
            {
                throw new EntityNotFoundException(gameModelEntity.Id, nameof(GameModelEntity).Replace("Entity", ""));
            }

            gameModelEntityToUpdate.Name = gameModelEntity.Name;
            gameModelEntityToUpdate.Description = gameModelEntity.Description;
            gameModelEntityToUpdate.Command1 = gameModelEntity.Command1;
            gameModelEntityToUpdate.Command2 = gameModelEntity.Command2;
            gameModelEntityToUpdate.Type = gameModelEntity.Type;
            gameModelEntityToUpdate.Width = gameModelEntity.Width;
            gameModelEntityToUpdate.Height = gameModelEntity.Height;
            gameModelEntityToUpdate.GroundItemVar = gameModelEntity.GroundItemVar;
            gameModelEntityToUpdate.ObjectModel = gameModelEntity.ObjectModel;
            gameModelEntityToUpdate.ModelId = gameModelEntity.ModelId;

            xmlDatabase.SaveEntities(gameModelEntities);
        }

        /// <summary>
        /// Removes the game model with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Remove(string id)
        {
            LoadEntitiesIfNeeded();

            gameModelEntities.RemoveAll(x => x.Id == id);

            try
            {
                xmlDatabase.SaveEntities(gameModelEntities);
            }
            catch
            {
                throw new DuplicateEntityException(id, nameof(GameModelEntity).Replace("Entity", ""));
            }
        }

        void LoadEntitiesIfNeeded()
        {
            if (loadedEntities)
            {
                return;
            }

            gameModelEntities = xmlDatabase.LoadEntities().ToList();
            loadedEntities = true;
        }
    }
}
