using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.Entities
{
    //Entidad base de todas las demás entidades
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }
}
