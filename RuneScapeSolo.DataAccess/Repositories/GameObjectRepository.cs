using System.Collections.Generic;
using System.IO;
using System.Linq;

using RuneScapeSolo.DataAccess.DataObjects;
using RuneScapeSolo.DataAccess.Exceptions;

namespace RuneScapeSolo.DataAccess.Repositories
{
    /// <summary>
    /// GameObject repository implementation.
    /// </summary>
    public class GameObjectRepository
    {
        readonly XmlDatabase<GameObjectEntity> xmlDatabase;
        List<GameObjectEntity> gameObjectEntities;
        bool loadedEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObjectRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public GameObjectRepository(string fileName)
        {
            xmlDatabase = new XmlDatabase<GameObjectEntity>(fileName);
            gameObjectEntities = new List<GameObjectEntity>();
        }

        public void ApplyChanges()
        {
            try
            {
                xmlDatabase.SaveEntities(gameObjectEntities);
            }
            catch
            {
                // TODO: Better exception message
                throw new IOException("Cannot save the changes");
            }
        }

        /// <summary>
        /// Adds the specified gameObject.
        /// </summary>
        /// <param name="gameObjectEntity">GameObject.</param>
        public void Add(GameObjectEntity gameObjectEntity)
        {
            LoadEntitiesIfNeeded();

            gameObjectEntities.Add(gameObjectEntity);
        }

        /// <summary>
        /// Get the gameObject with the specified identifier.
        /// </summary>
        /// <returns>The gameObject.</returns>
        /// <param name="id">Identifier.</param>
        public GameObjectEntity Get(string id)
        {
            LoadEntitiesIfNeeded();

            GameObjectEntity gameObjectEntity = gameObjectEntities.FirstOrDefault(x => x.Id == id);

            if (gameObjectEntity == null)
            {
                throw new EntityNotFoundException(id, nameof(GameObjectEntity).Replace("Entity", ""));
            }

            return gameObjectEntity;
        }

        /// <summary>
        /// Gets all the gameObjects.
        /// </summary>
        /// <returns>The gameObjects</returns>
        public IEnumerable<GameObjectEntity> GetAll()
        {
            LoadEntitiesIfNeeded();

            return gameObjectEntities;
        }

        /// <summary>
        /// Updates the specified gameObject.
        /// </summary>
        /// <param name="gameObjectEntity">GameObject.</param>
        public void Update(GameObjectEntity gameObjectEntity)
        {
            LoadEntitiesIfNeeded();

            GameObjectEntity gameObjectEntityToUpdate = gameObjectEntities.FirstOrDefault(x => x.Id == gameObjectEntity.Id);

            if (gameObjectEntityToUpdate == null)
            {
                throw new EntityNotFoundException(gameObjectEntity.Id, nameof(GameObjectEntity).Replace("Entity", ""));
            }

            gameObjectEntityToUpdate.Name = gameObjectEntity.Name;
            gameObjectEntityToUpdate.Description = gameObjectEntity.Description;
            gameObjectEntityToUpdate.Command1 = gameObjectEntity.Command1;
            gameObjectEntityToUpdate.Command2 = gameObjectEntity.Command2;
            gameObjectEntityToUpdate.Type = gameObjectEntity.Type;
            gameObjectEntityToUpdate.Width = gameObjectEntity.Width;
            gameObjectEntityToUpdate.Height = gameObjectEntity.Height;
            gameObjectEntityToUpdate.GroundItemVar = gameObjectEntity.GroundItemVar;
            gameObjectEntityToUpdate.ObjectModel = gameObjectEntity.ObjectModel;
            gameObjectEntityToUpdate.ModelId = gameObjectEntity.ModelId;

            xmlDatabase.SaveEntities(gameObjectEntities);
        }

        /// <summary>
        /// Removes the gameObject with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Remove(string id)
        {
            LoadEntitiesIfNeeded();

            gameObjectEntities.RemoveAll(x => x.Id == id);

            try
            {
                xmlDatabase.SaveEntities(gameObjectEntities);
            }
            catch
            {
                throw new DuplicateEntityException(id, nameof(GameObjectEntity).Replace("Entity", ""));
            }
        }

        void LoadEntitiesIfNeeded()
        {
            if (loadedEntities)
            {
                return;
            }

            gameObjectEntities = xmlDatabase.LoadEntities().ToList();
            loadedEntities = true;
        }
    }
}
