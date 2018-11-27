using System;

namespace Demo
{
    /// <summary>Представляет контаные данные, содержащие только почтовый адрес.</summary>
    public sealed class PostOnlyContact : Contact
    {
        /// <summary>Почтовый адрес.</summary>
        private readonly PostalContactInfo post_;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PostOnlyContact"/>.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="post">Почтовый адрес.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="name"/> иммет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="post"/> имеет значение <see langword="null"/>.
        /// </exception>
        public PostOnlyContact(PersonalName name, PostalContactInfo post)
            : base(name)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            post_ = post;
        }

        /// <inheritdoc />
        public override Contact UpdatePostalAddress(PostalContactInfo newPostalAddress)
            => new PostOnlyContact(Name, newPostalAddress);

        /// <inheritdoc />
        public override void AcceptVisitor(IContactVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(Name, post_);
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{base.ToString()}: Post: {post_}";
    }
}
