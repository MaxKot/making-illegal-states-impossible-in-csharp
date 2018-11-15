using System;

namespace Demo
{
    /// <summary>
    /// Представляет контаные данные, содержащие и адрес электронной почты, и почтовый адрес.
    /// </summary>
    public sealed class EmailAndPostContactInfo : ContactInfo
    {
        /// <summary>Адрес электронной почты.</summary>
        private readonly EmailContactInfo email_;

        /// <summary>Почтовый адрес.</summary>
        private readonly PostalContactInfo post_;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EmailAndPostContactInfo"/>.
        /// </summary>
        /// <param name="email">Адрес электронной почты.</param>
        /// <param name="post">Почтовый адрес.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="email"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="post"/> имеет значение <see langword="null"/>.
        /// </exception>
        public EmailAndPostContactInfo (EmailContactInfo email, PostalContactInfo post)
        {
            if (email == null)
            {
                throw new ArgumentNullException (nameof (email));
            }
            if (post == null)
            {
                throw new ArgumentNullException (nameof (post));
            }

            email_ = email;
            post_ = post;
        }

        /// <inheritdoc />
        public override void AcceptVisitor (IContactInfoVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException (nameof (visitor));
            }

            visitor.Visit (email_, post_);
        }

        /// <inheritdoc />
        public override string ToString ()
            => $"Email: {email_}; Post: {post_}";
    }
}
