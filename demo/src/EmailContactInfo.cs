using System;

namespace Demo
{
    /// <summary>Представляет контактный адрес электронной почты.</summary>
    public sealed class EmailContactInfo
    {
        /// <summary>
        /// Возвращает адрес электронной почты.
        /// </summary>
        /// <value>
        /// Адрес электронной почты.
        /// </value>
        public EmailAddress EmailAddress { get; }

        /// <summary>
        /// Возвращает флаг, обозначающий, проверен ли адрес электронной почты.
        /// </summary>
        /// <value>
        ///     <see langword="true"/>, если адрес электронной почты проверен;
        ///     иначе <see langword="false"/>.
        /// </value>
        public bool IsEmailVerified { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EmailContactInfo"/>.
        /// </summary>
        /// <param name="emailAddress">Адрес электронной почты.</param>
        /// <param name="isEmailVerified">
        /// <see langword="true"/>, если адрес электронной почты проверен.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="emailAddress"/> имеет значение <see langword="null"/>.
        /// </exception>
        public EmailContactInfo(EmailAddress emailAddress, bool isEmailVerified)
        {
            if (emailAddress == null)
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            EmailAddress = emailAddress;
            IsEmailVerified = isEmailVerified;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{EmailAddress}, {(IsEmailVerified ? "verified" : "not verified")}";
    }
}
