const API = 'http://localhost:5047/api/v1'

const ROOMS_FIXTURE = {
  items: [
    { roomName: 'Conference Room A', totalFaces: 5, scannedAt: '2025-01-15T10:00:00Z' },
    { roomName: 'Meeting Room B',    totalFaces: 2, scannedAt: '2025-01-14T09:30:00Z' },
    { roomName: 'Board Room',        totalFaces: 8, scannedAt: '2025-01-13T14:00:00Z' },
  ],
  totalPages: 1,
  currentPage: 1,
}

function stubCommonApis(rooms = ROOMS_FIXTURE) {
  cy.intercept('GET', `${API}/room/get*`, { statusCode: 200, body: rooms }).as('getRooms')
  cy.intercept('GET', `${API}/authentication/role`, { statusCode: 200, body: { role: 'Viewer' } }).as('getRole')
  cy.intercept('GET', `${API}/account/name`, { statusCode: 200, body: { accountName: 'Admin User' } }).as('getName')
}

describe('Rooms Page', () => {
  beforeEach(() => {
    cy.clearLocalStorage()
  })

  it('redirects unauthenticated users to /login', () => {
    cy.visit('/rooms')
    cy.url().should('include', '/login')
  })

  context('authenticated', () => {
    beforeEach(() => {
      stubCommonApis()
      cy.visit('/login')
      cy.window().then((win) => win.localStorage.setItem('token', 'fake-jwt-token'))
      cy.visit('/rooms')
      cy.wait('@getRooms')
    })

    it('displays the navbar with the RoomVision brand', () => {
      cy.contains('RoomVision').should('be.visible')
      cy.contains('Rooms').should('be.visible')
    })

    it('renders a card for each room', () => {
      cy.contains('Conference Room A').should('be.visible')
      cy.contains('Meeting Room B').should('be.visible')
      cy.contains('Board Room').should('be.visible')
    })

    it('shows the people count on each room card', () => {
      cy.contains('5').should('be.visible')
      cy.contains('2').should('be.visible')
      cy.contains('8').should('be.visible')
    })

    it('navigates to room info when a card is clicked', () => {
      cy.intercept('GET', `${API}/room/scan/result*`, {
        statusCode: 200,
        body: { totalFaces: 5, totalUnknown: 0, identifiedFaces: [], scannedAt: '2025-01-15T10:00:00Z' },
      }).as('getRoomInfo')
      cy.contains('Conference Room A').click()
      cy.url().should('include', '/room-info')
      cy.url().should('include', 'Conference')
    })

    it('does not show pagination when there is only one page', () => {
      cy.contains('Back').should('not.exist')
      cy.contains('Next').should('not.exist')
    })
  })

  context('pagination', () => {
    beforeEach(() => {
      cy.intercept('GET', `${API}/room/get*page=1*`, {
        statusCode: 200,
        body: { items: [{ roomName: 'Room 1', totalFaces: 1, scannedAt: '2025-01-01T00:00:00Z' }], totalPages: 2, currentPage: 1 },
      }).as('page1')
      cy.intercept('GET', `${API}/room/get*page=2*`, {
        statusCode: 200,
        body: { items: [{ roomName: 'Room 2', totalFaces: 2, scannedAt: '2025-01-01T00:00:00Z' }], totalPages: 2, currentPage: 2 },
      }).as('page2')
      cy.intercept('GET', `${API}/authentication/role`, { statusCode: 200, body: { role: 'Viewer' } })
      cy.intercept('GET', `${API}/account/name`, { statusCode: 200, body: { accountName: 'Admin' } })

      cy.visit('/login')
      cy.window().then((win) => win.localStorage.setItem('token', 'fake-jwt-token'))
      cy.visit('/rooms')
      cy.wait('@page1')
    })

    it('shows pagination controls when there are multiple pages', () => {
      cy.contains('Back').should('be.visible')
      cy.contains('Next').should('be.visible')
      cy.contains('1 / 2').should('be.visible')
    })

    it('loads the next page when Next is clicked', () => {
      cy.contains('Next').click()
      cy.wait('@page2')
      cy.contains('Room 2').should('be.visible')
      cy.contains('2 / 2').should('be.visible')
    })

    it('loads the previous page when Back is clicked', () => {
      cy.contains('Next').click()
      cy.wait('@page2')
      cy.contains('Back').click()
      cy.wait('@page1')
      cy.contains('Room 1').should('be.visible')
      cy.contains('1 / 2').should('be.visible')
    })
  })
})
