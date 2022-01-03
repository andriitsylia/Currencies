﻿namespace TelegramBot.Models
{

    public enum Currency : int
    {
        AUD = 036, // Австралийский доллар
        AZN = 944, // Азербайджанский манат
        ALL = 008, // Албанский лек
        DZD = 012, // Алжирский динар
        AOA = 973, // Ангольская кванза
        ARS = 032, // Аргентинское песо
        AMD = 051, // Армянский драм
        AWG = 533, // Арубанский флорин
        AFN = 971, // Афганский афгани
        BSD = 044, // Багамский доллар
        BDT = 050, // Бангладешская така
        BBD = 052, // Барбадосский доллар
        BHD = 048, // Бахрейнский динар
        BZD = 084, // Белизский доллар
        BYN = 933, // Белорусский рубль
        BMD = 060, // Бермудский доллар
        BGN = 975, // Болгарский лев
        BOB = 068, // Боливийский боливиано
        BWP = 072, // Ботсванская пула
        BRL = 986, // Бразильский реал
        BND = 096, // Брунейский доллар
        BIF = 108, // Бурундийский франк
        VUV = 548, // Вату Вануату
        HUF = 348, // Венгерский форинт
        KRW = 410, // Вона Республики Корея
        XCD = 951, // Восточно-карибский доллар
        VND = 704, // Вьетнамский донг
        HTG = 332, // Гаитянский гурд
        GYD = 328, // Гайанский доллар
        GMD = 270, // Гамбийский даласи
        GHS = 936, // Ганский седи
        GTQ = 320, // Гватемальский кетсаль
        GNF = 324, // Гвинейский франк
        GGP = 000, // Гернсийский фунт
        GIP = 292, // Гибралтарский фунт
        HNL = 340, // Гондурасская лемпира
        HKD = 344, // Гонконгский доллар
        GEL = 981, // Грузинский лари
        DKK = 208, // Датская крона
        MKD = 807, // Денар Республики Македония
        JEP = 000, // Джерсийский фунт
        AED = 784, // Дирхам ОАЭ
        STN = 930, // Добра Сан-Томе и Принсипи
        KID = 000, // Доллар Кирибати
        NAD = 516, // Доллар Намибии
        USD = 840, // Доллар США
        TVD = 000, // Доллар Тувалу
        DOP = 214, // Доминиканское песо
        EUR = 978, // ЕВРО
        EGP = 818, // Египетский фунт
        COU = 970, // Единица реальной стоимости
        ZMW = 967, // Замбийская квача новая
        INR = 356, // Индийская рупия
        IDR = 360, // Индонезийская рупия
        JOD = 400, // Иорданский динар
        IQD = 368, // Иракский динар
        IRR = 364, // Иранский риал
        ISK = 352, // Исландская крона
        YER = 886, // Йеменский риал
        KZT = 398, // Казахстанский тенге
        KYD = 136, // Каимановых островов доллар
        KHR = 116, // Камбоджийский риель
        CAD = 124, // Канадский доллар
        QAR = 634, // Катарский риал
        KES = 404, // Кенийский шиллинг
        PGK = 598, // Кина Папуа - Новая гвинея
        LAK = 418, // Кип Лаосской НДР
        KGS = 417, // Киргизский сом
        CNY = 156, // Китайский юань Жэньминьби
        COP = 170, // Колумбийский песо
        BAM = 977, // Конвертируемая марка
        CUC = 931, // Конвертируемое песо
        CDF = 976, // Конголезский франк
        CRC = 188, // Костариканский колон
        CUP = 192, // Кубинский песо
        KWD = 414, // Кувейтский динар
        MMK = 104, // Кьят Мьянма
        LRD = 430, // Либерийский доллар
        LBP = 422, // Ливанский фунт
        LYD = 434, // Ливийский динар
        LSL = 426, // Лоти Лесото
        MUR = 480, // Маврикийская рупия
        MRU = 929, // Мавританская угия
        MWK = 454, // Малавийская квача
        MGA = 969, // Малагасийский ариари
        MYR = 458, // Малайзийский ринггит
        MVR = 462, // Мальдивская руфия
        MAD = 504, // Марокканский дирхам
        MXN = 484, // Мексиканский песо
        MZN = 943, // Мозамбикский метикал
        MDL = 498, // Молдавский лей
        MNT = 496, // Монгольский тугрик
        BTN = 064, // Нгултрум Бутана
        NPR = 524, // Непальская рупия
        NGN = 566, // Нигерийская найра
        ANG = 532, // Нидерландский антильский гульден
        NIO = 558, // Никарагуанская золотая кордоба
        NZD = 554, // Новозеландский доллар
        ILS = 376, // Новый израильский шекель
        TWD = 901, // Новый тайваньский доллар
        TMT = 934, // Новый туркменский манат
        NOK = 578, // Норвежская крона
        OMR = 512, // Оманский риал
        PKR = 586, // Пакистанская рупия
        PAB = 590, // Панамский бальбоа
        PYG = 600, // Парагвайский гуарани
        MOP = 446, // Патака Макао
        PEN = 604, // Перуанский новый соль
        PLN = 985, // Польский злотый
        RUB = 643, // Российский рубль
        RWF = 646, // Руандийский франк
        RON = 946, // Румынский лей
        SVC = 222, // Сальвадорский колон
        WST = 882, // Самоа тала
        SAR = 682, // Саудовский риял
        SZL = 748, // Свазилендский лилангени
        XDR = 960, // СДР (спец. прав заим-я)
        KPW = 408, // Северокорейская вона
        SCR = 690, // Сейшельская рупия
        RSD = 941, // Сербский динар
        SGD = 702, // Сингапурский доллар
        SYP = 760, // Сирийский фунт
        SBD = 090, // Соломоновых островов доллар
        SOS = 706, // Сомалийский шиллинг
        VES = 928, // Суверенный боливар
        SDG = 938, // Суданский фунт
        SRD = 968, // Суринамский доллар
        SLL = 694, // Сьерра-Леонский леоне
        TJS = 972, // Таджикский сомони
        THB = 764, // Тайландский бат
        TZS = 834, // Танзанийский шиллинг
        TOP = 776, // Тонганская паанга
        TTD = 780, // Тринидада и Тобаго доллар
        TND = 788, // Тунисский динар
        TRY = 949, // Турецкая лира
        UGX = 800, // Угандийский шиллинг
        UZS = 860, // Узбекский сум
        UAH = 980, // Украинская гривна
        UYU = 858, // Уругвайские песо
        UYI = 940, // Уругвайское песо в индексированных единицах
        FJD = 242, // Фиджийский доллар
        PHP = 608, // Филиппинское песо
        FKP = 238, // Фолклендский фунт
        DJF = 262, // Франк Джибути
        KMF = 174, // Франк Коморских островов
        XAF = 950, // Франк КФА ВЕАС
        XOF = 952, // Франк КФА ВСЕАО
        XPF = 953, // Французский тихоокеанский Франк
        SHP = 654, // Фунт о-ва Святой Елены
        GBP = 826, // Фунт стерлингов
        HRK = 191, // Хорватская куна
        CZK = 203, // Чешская крона
        CLP = 152, // Чилийские песо
        SEK = 752, // Шведская крона
        CHF = 756, // Швейцарский франк
        LKR = 144, // Шри-ланкийская рупия
        ERN = 232, // Эритрейская накфа
        CVE = 132, // Эскудо Кабо-Верде
        ETB = 230, // Эфиопский быр
        ZAR = 710, // Южноафриканский рэнд
        SSP = 728, // Южносуданский фунт
        JMD = 388, // Ямайский доллар
        JPY = 392  // Японская йена

    }
}
