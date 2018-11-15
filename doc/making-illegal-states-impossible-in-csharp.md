Как сделать некорректные состояния невыразимыми на C#
---

Как правило статьи, рассказывающие о проктировании типами, содержат примеры на функциональных языках - Haskell, F# и других. Может показаться, что эта концепция неприменима к объекто-ориентированным языкам, но это не так.

В этой статье я переведу примеры из статьи Scott Wlaschin [Проектирование типами: Как сделать некорректные состояния невыразимыми](https://habr.com/post/424895/) на C#, при этом не сделав код [идиосинкразическим](http://blog.ploeh.dk/2015/08/03/idiomatic-or-idiosyncratic/). Также я постарась показать, что этот подход применим не только в качестве эксперимента, но и в рабочем коде.

<cut/>

# Создаём доменные типы

Примеры на F# базируются на [предыдущей статье серии](https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/), поэтому для начала я портирую типы из этой статьи.

## Оборачиваем примитивные типы в доменные

Примеры на F# используют доменные типы вместо примитивов для адреса электронной почты, почтового кода США и кода штата. Попробуем сделать обёртку примитивного типа на C#:

```csharp
public sealed class EmailAddress
{
    public string Value { get; }

    public EmailAddress (string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException (nameof (value));
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\S+@\S+\.\S+$"))
        {
            throw new ArgumentException ("Email address must contain an @ sign");
        }

        Value = value;
    }

    public override string ToString ()
        => Value;

    public override bool Equals (object other)
        => other is EmailAddress otherEmailAddress &&
           Value.Equals (otherEmailAddress.Value);

    public override int GetHashCode ()
        => Value.GetHashCode ();

    public static implicit operator string (EmailAddress address)
        => address?.Value;
}
```

```csharp
var a = new EmailAddress ("a@example.com");
var b = new EmailAddress ("b@example.com");

var receiverList = String.Join (";", a, b);
```

Я перенёс проверку корректности адреса из функции-фабрики в конструктор, поскольку такая реализация более типична для C#. Также пришлось реализовать сравнение и преобразование к строке, что на F# сделал бы компилятор.

С одной стороны, реализация выглядит довольно объёмной. С другой стороны, специфика адреса электронной почты выражена здесь только проверками в конструкторе и, возможно, логикой сравнения. Большую часть здесь занимает инфраструктурный код, который, к тому же, вряд ли будет меняться. Значит, можно либо [сделать](https://docs.microsoft.com/ru-ru/visualstudio/ide/walkthrough-creating-a-code-snippet?view=vs-2017) [шаблон](https://www.jetbrains.com/resharper/features/code_templates.html), либо, на худой конец, копировать общий код из класса в класс.

Надо отметить, что, создание доменных типов из примитивных значений - это не специфика функционального программирования. Наоборот, использование примитивных типов считается признаком [плохого кода в ООП](https://sourcemaking.com/refactoring/smells/primitive-obsession). Примеры таких обёрток можно у видеть, например, [в NLog](https://github.com/NLog/NLog/blob/v4.5.11/src/NLog/LogLevel.cs) и [в NBitcoin](https://github.com/MetacoSA/NBitcoin/blob/v4.1.1.68/NBitcoin/Money.cs#L268), да и стандартный тип TimeSpan - это, по сути обёртка над числом тиков.

## Создаём объекты-значения

Теперь надо создать аналог [записи](https://docs.microsoft.com/ru-ru/dotnet/fsharp/language-reference/records):

```csharp
public sealed class EmailContactInfo
{
    public EmailAddress EmailAddress { get; }

    public bool IsEmailVerified { get; }

    public EmailContactInfo (EmailAddress emailAddress, bool isEmailVerified)
    {
        if (emailAddress == null)
        {
            throw new ArgumentNullException (nameof (emailAddress));
        }

        EmailAddress = emailAddress;
        IsEmailVerified = isEmailVerified;
    }

    public override string ToString ()
        => $"{EmailAddress}, {(IsEmailVerified ? "verified" : "not verified")}";
}
```

Снова потребовалось больше кода, чем на F#, но большую часть работы можно выполнить за счёт [рефакторингов в IDE](https://docs.microsoft.com/ru-ru/visualstudio/ide/reference/generate-constructor?view=vs-2017).

Как и `EmailAddress`, `EmailContactInfo` - это [объект-значение](https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%BE%D0%B1%D0%BB%D0%B5%D0%BC%D0%BD%D0%BE-%D0%BE%D1%80%D0%B8%D0%B5%D0%BD%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%BD%D0%BE%D0%B5_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5#%D0%9E%D0%B1%D1%8A%D0%B5%D0%BA%D1%82-%D0%B7%D0%BD%D0%B0%D1%87%D0%B5%D0%BD%D0%B8%D0%B5) (в смысле DDD, а не [типы-значения в .NET](https://docs.microsoft.com/ru-ru/dotnet/csharp/language-reference/keywords/value-types)), давно известный и применяемый в объектом моделировании.

Остальные типы - `StateCode`, `ZipCode`, `PostalAddress` и `PersonalName` портируются на C# схожим образом.

# Создаём тип-сумму

На первый взгляд, тип-сумма `ContactInfo` принадлежит миру функционального программирования и не переносится на объектно-ориентированные языки никаким адекватным образом. Но прежде чем бросать затею с невыразимостью некорректных состояний и писать проверки в конструкторе, всё же стоит изучить возможные варианты.

## Инструменты для реализации в C#

Самым очевидным выражением взаимоисключающих состояний в C# является enum. К сожалению, в этой задаче разным состояниям присущи также различные данные, так что придётся создать разные типы с общим базовым классом.

```csharp
[Flags]
public enum ContactInfoState
{
    EmailOnly = 1,
    PostOnly = 2,
    EmailAndPost = EmailOnly | PostOnly
}

public abstract class ContactInfo
{

}

// Конструкторы убраны для краткости

public sealed class EmailOnlyContactInfo : ContactInfo
{
    public EmailContactInfo Email { get; }
}

public sealed class PostOnlyContactInfo : ContactInfo
{
    public PostalContactInfo Post { get; }
}

public sealed class EmailAndPostContactInfo : ContactInfo
{
    public EmailContactInfo Email { get; }

    public PostalContactInfo Post { get; }
}
```

Теперь можно передавать `ContactInfoState` и `ContactInfo` и выполнять преобразование контакта к нужному типу в зависимости от состояния флагов. Наверное, работать будет, но оставляет открытыми много вопросов: значение `ContactInfoState` по умолчанию не определено, нет гарантий, что будут установлены корректные флаги состояния, да и соответствие флагов состояния и переданного `ContactInfo` под вопросом.

А что если сделать `ContactInfoState` членом `ContactInfo` и инициализировать в онструторе наследников? Тогда вопросы 2 и 3 будут решены. С дургой стороны, зачем отдельно хранить флаги, если можно напрямую проверять тип, теме более что теперь в C# есть ограниченная поддержка сопоставления с образцом?

```csharp
switch (contactInfo)
{
    case EmailOnlyContactInfo emailContactInfo:
        ...
    case PostOnlyContactInfo postContactInfo:
        ...
    case EmailAndPostContactInfo emailAndPostContactInfo:
        ...
}
```

Уже лучше, лишь бы не забыть какой-нибудь switch по реализациям `ContactInfo` при добавлении наследников.

А есть ли лучший способ выбрать действие в зависимости от типа в ООП вообще и C# в частности? В такой постановке вопроса ответ очевиден - надо сделать `ContactInfo` полиморфным, добавив виртуальные методы в базовый класс. Тогда код не только станет более идиоматичным, но и исключит возможность забыть про обработку новых состояний контакта, если они появятся.

Единственная проблема - скорее всего, контакт будет использоваться во многих местах, и, если для каждого места делать свой виртуальный метод, то границы ответственности иерархии `ContactInfo` будет размываться, а логика различных случаев использования контактов размазываться между собственно обработкой и наследниками `ContactInfo`. Впрочем, до решения этой проблемы и окончательного варианта реализации типа-суммы в C# остался один шаг.

## Visitor (посетитель)

Старый шаблон проектирования Посетитель отвечает всем требованиям. Он позволяется реализовать только допустимые состояния контакта, не вынуждает сваливать все возможные случаи обработи контакта в его наследников и позволяет переложить проверку полноты обработки состояний контакта на компилятор.

```csharp
public abstract class ContactInfo
{
    public abstract void AcceptVisitor (IContactInfoVisitor visitor);
}

public interface IContactInfoVisitor
{
    void Visit (EmailContactInfo email);

    void Visit (PostalContactInfo post);

    void Visit (EmailContactInfo email, PostalContactInfo post);
}

public sealed class EmailOnlyContactInfo : ContactInfo
{
    private readonly EmailContactInfo email_;

    public override void AcceptVisitor (IContactInfoVisitor visitor)
        => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (email_);
}

public sealed class PostOnlyContactInfo : ContactInfo
{
    private readonly PostalContactInfo post_;

    public override void AcceptVisitor (IContactInfoVisitor visitor)
        => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (post_);
}

public sealed class EmailAndPostContactInfo : ContactInfo
{
    private readonly EmailContactInfo email_;

    private readonly PostalContactInfo post_;

    public override void AcceptVisitor (IContactInfoVisitor visitor)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException (nameof (visitor));
        }

        visitor.Visit (email_, post_);
    }
}
```

Я сделал поля реализаций `ContactInfo` закрытыми, чтобы проверка типа объекта без использования `IContactInfoVisitor` была бессмысленной.

Теперь можно реализовать создание контакта:

```csharp
public sealed class Contact
{
    public PersonalName Name { get; }

    public ContactInfo ContactInfo { get; }

    public Contact (PersonalName name, ContactInfo contactInfo)
    {
        if (name == null)
        {
            throw new ArgumentNullException (nameof (name));
        }
        if (contactInfo == null)
        {
            throw new ArgumentNullException (nameof (contactInfo));
        }

        Name = name;
        ContactInfo = contactInfo;
    }

    public static Contact FromEmail (PersonalName name, string emailStr)
    {
        var email = new EmailAddress (emailStr);
        var emailContactInfo = new EmailContactInfo (email, false);
        var contactInfo = new EmailOnlyContactInfo (emailContactInfo);
        return new Contact (name, contactInfo);
    }
}
```

И обновление:

```csharp
public sealed class Contact
{
    private sealed class PostalAddressUpdater : IContactInfoVisitor
    {
        public ContactInfo UpdatedContactInfo { get; private set; }

        private readonly PostalContactInfo newPostalAddress_;

        public PostalAddressUpdater (PostalContactInfo newPostalAddress)
        {
            System.Diagnostics.Debug.Assert (newPostalAddress != null);
            newPostalAddress_ = newPostalAddress;
        }

        public void Visit (EmailContactInfo email)
        {
            System.Diagnostics.Debug.Assert (email != null);
            UpdatedContactInfo = new EmailAndPostContactInfo (email, newPostalAddress_);
        }

        public void Visit (PostalContactInfo _)
        {
            UpdatedContactInfo = new PostOnlyContactInfo (newPostalAddress_);
        }

        public void Visit (EmailContactInfo email, PostalContactInfo _)
        {
            System.Diagnostics.Debug.Assert (email != null);
            UpdatedContactInfo = new EmailAndPostContactInfo (email, newPostalAddress_);
        }
    }

    public Contact UpdatePostalAddress (PostalContactInfo newPostalAddress)
    {
        var updater = new PostalAddressUpdater (newPostalAddress);
        ContactInfo.AcceptVisitor (updater);
        return new Contact (Name, updater.UpdatedContactInfo);
    }
}
```

## Пример использования

Ну и наконец, воспроизведём пример использования получившегося кода:

```csharp
var name = new PersonalName ("A", null, "Smith");
var contact = Contact.FromEmail (name, "abc@example.com");

var state = new StateCode ("CA");
var zip = new ZipCode ("97210");
var newPostalAddress = new PostalAddress ("123 Main", "", "Beverly Hills", state, zip);
var newPostalContactInfo = new PostalContactInfo (newPostalAddress, false);
var newContact = contact.UpdatePostalAddress (newPostalContactInfo);
```

Как и при использовании option.Value в F#, здесь возможен выброс исключения из конструкторов, если адрес электронной почты, почтовый индекс или штат указаны неверно, но для C# это является распространённой практикой. Конечно же, в рабочем коде здесь или где-то в вызывающем коде должна быть предусмотрена обработка исключений.

## Дальнейшие улучшения

При переносе примеров с F# я старался использовать наиболее распространённые варианты реализации.

Так, я использовал "классическую" реализацию Посетителя, изменяющую своё внутреннее состояние. Можно немного видоизменить шаблон, чтобы Посетитель возвращал значение.

```csharp
public abstract class ContactInfo
{
    public abstract T AcceptVisitor<T> (IContactInfoVisitor<T> visitor);
}

public interface IContactInfoVisitor<T>
{
    T Visit (EmailContactInfo email);

    T Visit (PostalContactInfo post);

    T Visit (EmailContactInfo email, PostalContactInfo post);
}
```

Во-вторых, если `ContactInfo` объявлен в библиотеке и появление новых наследников в клиентах бибилиотеки нежелательно, то можно добавить конструктор ContactInfo с видимостью internal, либо вообще сделать его наследников вложенными классами, объявить видимость конструктора и наследников private, а создание эземпляров делать через статические методы-фабрики.

```csharp
public abstract class ContactInfo
{
    private sealed class EmailOnlyContactInfo : ContactInfo
    {
        private readonly EmailContactInfo email_;
    }

    private ContactInfo ()
    {

    }

    public static ContactInfo EmailOnly (EmailContactInfo email)
        => new EmailOnlyContactInfo (email);
}
```

Таким образом можно воспроизвести нерасширяемость типа-суммы, хотя, как правило, этого не требуется.

# Залючение

Надеюсь, мне удалось показать, как средствами ООП ограничить корректные состояния бизнес-логики при помощи типов. Код получился более объёмным, чем на F#. Где-то это обусловлено относительной громоздкостью решений ООП, где-то - многословностью языка, но решения нельзя назвать непрактичными.

Что интересно, начав с чисто функционального решения, мы пришли к следованию рекомендация предметно-ориентированного программирования и паттернам ООП. На самом деле, это не удивительно, потому что подобие типов-сумм и паттерна Посетитель [известно](http://blog.ploeh.dk/2018/06/25/visitor-as-a-sum-type/), и довольно [давно](http://www.drdobbs.com/cpp/discriminated-unions-i/184403821). Целью этой статьи было показать не столько конкретный приём, сколько продемонстрировать применимость идей из ["башни из слоновой кости"](https://fsharpforfunandprofit.com/fppatterns/#twitterstorm) в императивном программировании. Конечно, не всё получится перенести так же легко, но с появлением всё новых и новых функциональных возможностей в мейнстримовых языках программирования границы применимого будут расширятся.
