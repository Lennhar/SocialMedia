using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    //Reglas de negocio (Business logic) | Puede ser una proyecto adicional (ctrl+R+I genera la interfaz )
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> GetPostById(int id)
        {
            return await _unitOfWork.PostRepository.GetById(id);
        }

        public IEnumerable<Post> GetPosts(PostQueryFilter filter)
        {
            var posts = _unitOfWork.PostRepository.GetAll();
            if (filter.UserId != null)
            {
                posts = posts.Where(x=>x.UserId == filter.UserId);
            }
            if (filter.Date != null)
            {
                posts = posts.Where(x => x.Date.ToShortDateString() == filter.Date?.ToShortDateString());
            }
            if (filter.Description != null)
            {
                posts = posts.Where(x => x.Description.ToLower().Contains(filter.Description.ToLower()));
            }
            return posts;
        }

        //Solo se permite publicar a un usuario previamente registrado y que los post no contengan sexo
        public async Task InsertPost(Post post)
        {
            var user = await _unitOfWork.UserRepository.GetById(post.Id);
            if (user == null)
            {
                throw new BussinessException("User does not exist");
            }

            var userPost = await _unitOfWork.PostRepository.GetPostsByUser(post.UserId);
            //Si el usuario tiene menos de 10 post solo puede comentar 1 vez cada 7 días
            if (userPost.Count() > 10)
            {
                var lastPost = userPost.OrderByDescending(x => x.Date).FirstOrDefault();

                if ((DateTime.Now - lastPost.Date).TotalDays < 7)
                {
                    throw new BussinessException("you're no able to publish the post");
                }
            }

            if (post.Description.Contains("Sexo") || post.Description.Contains("sexo"))
            {
                throw new BussinessException("Contenido no permitido");
            }
            //Fecha actual del post
            post.Date = DateTime.Now;

            await _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveChangesASync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveChangesASync();
            return true;
        }

        public async Task<bool> DeletePost(int id)
        {
            await _unitOfWork.PostRepository.Delete(id);
            return true;
        }
    }
}
