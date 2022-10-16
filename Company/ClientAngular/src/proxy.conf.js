const { debug } = require("console");

const PROXY_CONFIG = [
  {
    context: [
      "/WWWWWWpi/",
    ],
    target: "http://localhost:3602/v1.0/invoke/FrontApiStockMarket/method/",
    secure: false,
    "logLevel": "debug"
  }
]

module.exports = PROXY_CONFIG;
