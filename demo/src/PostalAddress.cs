using System;

namespace Demo
{
    /// <summary>Представляет почтовый адрес.</summary>
    public sealed class PostalAddress
    {
        /// <summary>
        /// Возвращает первую строку адреса.
        /// </summary>
        /// <value>Первая строка адреса.</value>
        public string Address1 { get; }

        /// <summary>
        /// Возвращает вторую строку адреса.
        /// </summary>
        /// <value>Вторая строка адреса.</value>
        public string Address2 { get; }

        /// <summary>
        /// Возвращает город.
        /// </summary>
        /// <value>Город.</value>
        public string City { get; }

        /// <summary>
        /// Возвращает штат.
        /// </summary>
        /// <value>Штат.</value>
        public StateCode State { get; }

        /// <summary>
        /// Возвращает почтовый индекс.
        /// </summary>
        /// <value>Почтовый индекс.</value>
        public ZipCode Zip { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PostalAddress"/>.
        /// </summary>
        /// <param name="address1">Первая строка адреса.</param>
        /// <param name="address2">Вторая строка адреса.</param>
        /// <param name="city">Город.</param>
        /// <param name="state">Штат.</param>
        /// <param name="zip">Почтовый индекс.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="address1"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="address2"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="city"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="state"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="zip"/> имеет значение <see langword="null"/>.
        /// </exception>
        public PostalAddress
            (string address1, string address2, string city, StateCode state, ZipCode zip)
        {
            if (address1 == null)
            {
                throw new ArgumentNullException(nameof(address1));
            }
            if (address2 == null)
            {
                throw new ArgumentNullException(nameof(address2));
            }
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (zip == null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            Zip = zip;
        }

        /// <inheritdoc />
        public override string ToString()
            => !String.IsNullOrEmpty(Address2)
                ? $"{Address1} {Address2}, {City}, {State} {Zip}"
                : $"{Address1}, {City}, {State} {Zip}";
    }
}
