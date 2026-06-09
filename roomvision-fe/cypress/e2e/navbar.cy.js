const API = 'http://localhost:5047/api/v1'

function setupAuthenticated(role = 'Viewer') {
  cy.intercept('GET', `${API}/room/get*`, { statusCode: 200, body: { items: [], totalPages: 1 } }).as('getRooms')
  cy.intercept('GET', `${API}/authentication/role`, { statusCode: 200, body: { role } }).as('getRole')
  cy.intercept('GET', `${API}/account/name`, { statusCode: 200, body: { accountName: 'Jane Doe' } }).as('getName')

  cy.visit('/login')
  cy.window().then((win) => win.localStorage.setItem('token', 'fake-jwt-token'))
  cy.visit('/rooms')
  cy.wait('@getRooms')
}

describe('Navbar', () => {
  beforeEach(() => {
    cy.clearLocalStorage()
  })

  context('viewer role', () => {
    beforeEach(() => {
      setupAuthenticated('Viewer')
    })

    it('displays brand and Rooms nav link', () => {
      cy.contains('RoomVision').should('be.visible')
      cy.contains('Rooms').should('be.visible')
    })

    it('opens the dropdown menu when avatar is clicked', () => {
      cy.wait('@getName')
      cy.contains('JD').click()
      cy.contains('Change password').should('be.visible')
      cy.contains('Add person').should('be.visible')
      cy.contains('Sign out').should('be.visible')
    })

    it('does not show Administrators option for viewer', () => {
      cy.wait('@getName')
      cy.contains('JD').click()
      cy.contains('Administrators').should('not.exist')
    })

    it('logs out and redirects to login when Sign out is clicked', () => {
      cy.wait('@getName')
      cy.contains('JD').click()
      cy.contains('Sign out').click()
      cy.url().should('include', '/login')
      cy.window().its('localStorage').invoke('getItem', 'token').should('be.null')
    })
  })

  context('admin role', () => {
    beforeEach(() => {
      setupAuthenticated('Admin')
    })

    it('shows Administrators option for admin users', () => {
      cy.wait('@getName')
      cy.contains('JD').click()
      cy.contains('Administrators').should('be.visible')
    })
  })

  context('invalid token', () => {
    it('redirects to /login when role API returns 401', () => {
      cy.intercept('GET', `${API}/room/get*`, { statusCode: 200, body: { items: [], totalPages: 1 } })
      cy.intercept('GET', `${API}/authentication/role`, { statusCode: 401, body: {} }).as('getRole401')
      cy.intercept('GET', `${API}/account/name`, { statusCode: 200, body: { accountName: 'Admin' } })

      cy.visit('/login')
      cy.window().then((win) => win.localStorage.setItem('token', 'expired-token'))
      cy.visit('/rooms')
      cy.wait('@getRole401')
      cy.url().should('include', '/login')
    })
  })
})
