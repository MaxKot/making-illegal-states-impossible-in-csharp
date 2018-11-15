using System;

namespace Demo
{
    /// <summary>Представляет контаные данные, содержащие только почтовый адрес.</summary>
    public sealed class PostOnlyContactInfo : ContactInfo
    {
        /// <summary>Почтовый адрес.</summary>
        private readonly PostalContactInfo post_;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PostOnlyContactInfo"/>.
        /// </summary>
        /// <param name="post">Почтовый адрес.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="post"/> имеет значение <see langword="null"/>.
        /// </exception>
        public PostOnlyContactInfo (PostalContactInfo post)
        {
            if (post == null)
            {
                throw new ArgumentNullException (nameof (post));
            }

            post_ = post;
        }

        /// <inheritdoc />
        public override void AcceptVisitor (IContactInfoVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException (nameof (visitor));
            }

            visitor.Visit (post_);
        }

        /// <inheritdoc />
        public override string ToString ()
            => $"Post: {post_}";
    }
}
