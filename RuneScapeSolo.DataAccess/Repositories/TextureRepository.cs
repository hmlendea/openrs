using System.Collections.Generic;
using System.IO;
using System.Linq;

using RuneScapeSolo.DataAccess.DataObjects;
using RuneScapeSolo.DataAccess.Exceptions;

namespace RuneScapeSolo.DataAccess.Repositories
{
    /// <summary>
    /// Texture repository implementation.
    /// </summary>
    public class TextureRepository
    {
        readonly XmlDatabase<TextureEntity> xmlDatabase;
        List<TextureEntity> textureEntities;
        bool loadedEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public TextureRepository(string fileName)
        {
            xmlDatabase = new XmlDatabase<TextureEntity>(fileName);
            textureEntities = new List<TextureEntity>();
        }

        public void ApplyChanges()
        {
            try
            {
                xmlDatabase.SaveEntities(textureEntities);
            }
            catch
            {
                // TODO: Better exception message
                throw new IOException("Cannot save the changes");
            }
        }

        /// <summary>
        /// Adds the specified texture.
        /// </summary>
        /// <param name="textureEntity">Texture.</param>
        public void Add(TextureEntity textureEntity)
        {
            LoadEntitiesIfNeeded();

            textureEntities.Add(textureEntity);
        }

        /// <summary>
        /// Get the texture with the specified identifier.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="id">Identifier.</param>
        public TextureEntity Get(string id)
        {
            LoadEntitiesIfNeeded();

            TextureEntity textureEntity = textureEntities.FirstOrDefault(x => x.Id == id);

            if (textureEntity == null)
            {
                throw new EntityNotFoundException(id, nameof(TextureEntity).Replace("Entity", ""));
            }

            return textureEntity;
        }

        /// <summary>
        /// Gets all the textures.
        /// </summary>
        /// <returns>The textures</returns>
        public IEnumerable<TextureEntity> GetAll()
        {
            LoadEntitiesIfNeeded();

            return textureEntities;
        }

        /// <summary>
        /// Updates the specified texture.
        /// </summary>
        /// <param name="textureEntity">Texture.</param>
        public void Update(TextureEntity textureEntity)
        {
            LoadEntitiesIfNeeded();

            TextureEntity textureEntityToUpdate = textureEntities.FirstOrDefault(x => x.Id == textureEntity.Id);

            if (textureEntityToUpdate == null)
            {
                throw new EntityNotFoundException(textureEntity.Id, nameof(TextureEntity).Replace("Entity", ""));
            }

            textureEntityToUpdate.Name = textureEntity.Name;
            textureEntityToUpdate.SubName = textureEntity.SubName;

            xmlDatabase.SaveEntities(textureEntities);
        }

        /// <summary>
        /// Removes the texture with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Remove(string id)
        {
            LoadEntitiesIfNeeded();

            textureEntities.RemoveAll(x => x.Id == id);

            try
            {
                xmlDatabase.SaveEntities(textureEntities);
            }
            catch
            {
                throw new DuplicateEntityException(id, nameof(TextureEntity).Replace("Entity", ""));
            }
        }

        void LoadEntitiesIfNeeded()
        {
            if (loadedEntities)
            {
                return;
            }

            textureEntities = xmlDatabase.LoadEntities().ToList();
            loadedEntities = true;
        }
    }
}
