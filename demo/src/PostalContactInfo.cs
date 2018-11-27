using System;

namespace Demo
{
    /// <summary>Представляет контактный почтовый адрес.</summary>
    public sealed class PostalContactInfo
    {
        /// <summary>
        /// Возвращает почтовый адрес.
        /// </summary>
        /// <value>Почтовый адрес.</value>
        public PostalAddress Address { get; }

        /// <summary>
        /// Возвращает флаг, обозначающий, проверен ли почтовый адрес.
        /// </summary>
        /// <value>
        ///     <see langword="true"/>, если почтовый адрес проверен;
        ///     иначе <see langword="false"/>.
        /// </value>
        public bool IsAddressValid { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PostalContactInfo"/>.
        /// </summary>
        /// <param name="address">Почтовый адрес.</param>
        /// <param name="isEmailVerified">
        /// <see langword="true"/>, если почтовый адрес.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="address"/> имеет значение <see langword="null"/>.
        /// </exception>
        public PostalContactInfo(PostalAddress address, bool isAddressValid)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
            IsAddressValid = isAddressValid;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{Address}, {(IsAddressValid ? "valid" : "invalid")}";
    }
}
