const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
    ],
    target: "https://localhost:7081",
    secure: false
  },
  {
    context: [
      "/api",
    ],
    target: "https://www.api.trainingday.space",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
