﻿using InfoHub.Core.Dtos;
using InfoHub.Core.Helpers;
using InfoHub.Core.Interfaces;
using InfoHub.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace InfoHub.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IRepository<UserPoint> _userPointRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, IRepository<UserPoint> userPointRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _postRepository = postRepository;
            _userPointRepository = userPointRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        public async Task<PagedList<Post>> GetAllPostsAsync(PaginationParameters paginationParameters)
        {
            var posts = await _postRepository.GetAllPostsAsync();
            return PagedList<Post>.ToPagedList(posts.AsQueryable(), paginationParameters.PageNumber, paginationParameters.PageSize);
        }
       
        public Post GetPost(int id)
        {
            var post = _postRepository.GetPost(id);
            return post;
        }
        
        public Post AddPost(PostDto post)
        {
            var postMap = _mapper.Map<Post>(post);
            _postRepository.Add(postMap);
            _unitOfWork.Complete();
            return postMap;
        }

        public void DeletePost(int postId)
        {
            var post = _postRepository.Get(x => x.Id == postId);
            _postRepository.Delete(post);
            _unitOfWork.Complete();
        }
        
        public void UpdatePost(PostDto post)
        {
            var postMap = _mapper.Map<Post>(post);
            _postRepository.Update(postMap);
            _unitOfWork.Complete();
        }
        
        public void UpVote(int userId, int id)
        {
            var userPoint = new UserPoint { UserId = userId, PostId = id };
            _userPointRepository.Add(userPoint);
            _unitOfWork.Complete();
        }

        public void DownVote(int userId, int id)
        {
            var userPoint = _userPointRepository.Get(x => x.UserId == userId && x.PostId == id);
            _userPointRepository.Delete(userPoint);
            _unitOfWork.Complete();
        }
    }
}
