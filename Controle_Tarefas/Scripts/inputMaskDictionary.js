var currency = {
    "prefix": "R$ ",
    "alias": "currency",
    "groupSeparator": ".",
    "isNumeric": true,
    "digitsOptional": true,
    "min": 0,
    "max": 999999999999999999,
    "radixPoint": ","
}

var diasInput = {
    "suffix": " dias",
    "alias": "numeric",
    "isNumeric": true,
    "digitsOptional": false,
    "placeholder": " dias",
    "min": 0,
    "max": 999
}

var currencySemDecimals = {
    "prefix": "R$ ",
    "alias": "currency",
    "groupSeparator": ".",
    "isNumeric": true,
    "digits": 2 ,
    "allowMinus": true,
    "min": -999999999999999999,
    "max": 999999999999999999,
    "radixPoint": ","
}

var currencyTotal = {
    "prefix": "R$ ",
    "alias": "currency",
    "groupSeparator": ".",
    "isNumeric": true,
    "digitsOptional": true,
    "max": 999999999999999999,
    "radixPoint": ","
}

var currencyTotalNeg = {
    "prefix": "R$ ",
    "alias": "currency",
    "groupSeparator": ".",
    "isNumeric": true,
    "digits": 2,
    "digitsOptional": true,
    "min": -999999999999999999,
    "max": 999999999999999999,
    "radixPoint": ","
}

var currency16Decimals = {
    "prefix": "R$ ",
    "alias": "currency",
    "groupSeparator": ".",
    "isNumeric": true,
    "digits": 16,
    "max": 9999,
    "radixPoint": ".",
    "integerdigits": 4
}

var currencyTotalDecimals3 = {
    "prefix": "R$ ",
    "alias": "currency",
    "groupSeparator": ".",
    "isNumeric": true,
    "digits": 3,
    "digitsOptional": !1,
    "max": 999999999999999999,
    "radixPoint": ","
}

var perc2 = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 2,
    "digitsOptional": true,
    "rightAlign": false,
    "integerDigits": 1,
    "min": 0,
    "max": 10,
    "radixPoint": ","
}

var perc2Neg = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 2,
    "digitsOptional": true,
    "rightAlign": false,
    "integerDigits": 1,
    "min": -10,
    "max": 10,
    "radixPoint": ",",
    "placeholder": " "
}

var perc3 = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 2,
    "digitsOptional": true,
    "rightAlign": true,
    "integerDigits": 3,
    "min": 0,
    "max": 100,
    "radixPoint": ","
}

var perc3PosNeg = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 2,
    "digitsOptional": true,
    "rightAlign": true,
    "integerDigits": 3,
    "placeholder": " ",
    "min": -100,
    "max": 100,
    "radixPoint": ","
}

var porc4dec = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 4,
    "rightAlign": true,
    "integerDigits": 3,
    "min": 0,
    "max": 100,
    "radixPoint": ","
}

var perc3noLimit = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 2,
    "digitsOptional": true,
    "rightAlign": true,
    "integerDigits": 3,
    "min": 0,
    "max": 1000,
    "radixPoint": ","
}

var perc3noLimitNeg = {
    "alias": "numeric",
    "groupSeparator": ".",
    "isNumeric": true,
    "suffix": "%",
    "autoGroup": true,
    "digits": 2,
    "digitsOptional": true,
    "rightAlign": true,
    "integerDigits": 4,
    "min": -1000,
    "max": 1000,
    "radixPoint": ",",
    "placeholder": " "
}