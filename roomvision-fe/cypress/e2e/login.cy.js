const API = 'http://localhost:5047/api/v1'

describe('Login Page', () => {
  beforeEach(() => {
    cy.clearLocalStorage()
    cy.visit('/login')
  })

  it('renders the login form', () => {
    cy.get('#email').should('exist')
    cy.get('#password').should('exist')
    cy.get('button[type="submit"]').should('contain', 'Enter dashboard')
  })

  it('shows error when submitting empty form', () => {
    cy.get('button[type="submit"]').click()
    cy.contains('Please fill in all fields').should('be.visible')
  })

  it('shows error when only email is filled', () => {
    cy.get('#email').type('admin@roomvision.com')
    cy.get('button[type="submit"]').click()
    cy.contains('Please fill in all fields').should('be.visible')
  })

  it('shows error when only password is filled', () => {
    cy.get('#password').type('secret')
    cy.get('button[type="submit"]').click()
    cy.contains('Please fill in all fields').should('be.visible')
  })

  it('toggles password visibility', () => {
    cy.get('#password').should('have.attr', 'type', 'password')
    cy.contains('Show').click()
    cy.get('#password').should('have.attr', 'type', 'text')
    cy.contains('Hide').click()
    cy.get('#password').should('have.attr', 'type', 'password')
  })

  it('shows an error message on failed login', () => {
    cy.intercept('POST', `${API}/authentication/user`, {
      statusCode: 401,
      body: 'Invalid credentials',
    }).as('loginFail')

    cy.get('#email').type('wrong@example.com')
    cy.get('#password').type('wrongpassword')
    cy.get('button[type="submit"]').click()
    cy.wait('@loginFail')

    // When the API throws a string, err?.message is undefined → falls back to this
    cy.contains('Login failed. Please try again.').should('be.visible')
  })

  it('redirects to /rooms after successful login', () => {
    cy.intercept('POST', `${API}/authentication/user`, {
      statusCode: 200,
      body: { token: 'fake-jwt-token' },
    }).as('loginSuccess')

    cy.intercept('GET', `${API}/room/get*`, { statusCode: 200, body: { items: [], totalPages: 1 } })
    cy.intercept('GET', `${API}/authentication/role`, { statusCode: 200, body: { role: 'Viewer' } })
    cy.intercept('GET', `${API}/account/name`, { statusCode: 200, body: { accountName: 'Admin User' } })

    cy.get('#email').type('admin@roomvision.com')
    cy.get('#password').type('correctpassword')
    cy.get('button[type="submit"]').click()
    cy.wait('@loginSuccess')

    cy.url().should('include', '/rooms')
    cy.window().its('localStorage').invoke('getItem', 'token').should('eq', 'fake-jwt-token')
  })

  it('redirects already-authenticated users away from login', () => {
    // Token must be set on the correct origin — visit the page first
    cy.intercept('GET', `${API}/room/get*`, { statusCode: 200, body: { items: [], totalPages: 1 } })
    cy.intercept('GET', `${API}/authentication/role`, { statusCode: 200, body: { role: 'Viewer' } })
    cy.intercept('GET', `${API}/account/name`, { statusCode: 200, body: { accountName: 'Admin' } })

    cy.window().then((win) => win.localStorage.setItem('token', 'existing-token'))
    cy.visit('/login')
    cy.url().should('include', '/rooms')
  })
})
