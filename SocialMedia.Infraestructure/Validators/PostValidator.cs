using FluentValidation;
using SocialMedia.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infraestructure.Validators
{
    //Se usa la librería fluentValidator
    public class PostValidator : AbstractValidator<PostDTO>
    {
        public PostValidator()
        {
            RuleFor(Post => Post.Description)
                .NotNull()
                .Length(10, 25);

            RuleFor(Post => Post.Date)
                .NotNull()
                .LessThan(DateTime.Now);
        }
    }
}
