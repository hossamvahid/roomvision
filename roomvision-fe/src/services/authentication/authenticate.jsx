import api from '../api'
export  default  async function authenticate(email, password){
	try {
		const response = await api.post('/authentication/user', {
			email,
			password
		})

		if (response.data.token) {
			localStorage.setItem('token', response.data.token)
		}

		return response.data
	} catch (error) {
		throw error.response?.data || error.message
	}
}
