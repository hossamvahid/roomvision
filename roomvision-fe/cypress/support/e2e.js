const API = 'http://localhost:5047/api/v1'

Cypress.Commands.add('login', (email = 'admin@roomvision.com', password = 'password') => {
  cy.intercept('POST', `${API}/authentication/user`, {
    statusCode: 200,
    body: { token: 'fake-jwt-token' },
  }).as('loginRequest')

  cy.visit('/login')
  cy.get('#email').type(email)
  cy.get('#password').type(password)
  cy.get('button[type="submit"]').click()
  cy.wait('@loginRequest')
})

// Sets a token in localStorage by visiting the app first (correct origin)
Cypress.Commands.add('setToken', (token = 'fake-jwt-token') => {
  cy.visit('/')
  cy.window().then((win) => {
    win.localStorage.setItem('token', token)
  })
})
