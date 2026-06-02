import { Box } from '@mui/material'
import { LoginBox } from '../components/login'
import VisualPane from '../components/login/VisualPane'

export default function Login() {
	return (
		<Box
			sx={{
				display: 'flex',
				minHeight: '100vh',
				width: '100%',
				bgcolor: 'oklch(0.135 0.006 248)',
				'@media (max-width: 900px)': {
					flexDirection: 'column',
				},
			}}
		>
			{/* Form pane */}
			<Box
				component="section"
				sx={{
					flex: '0 0 46%',
					maxWidth: '620px',
					minWidth: '380px',
					bgcolor: 'oklch(0.155 0.006 248)',
					display: 'flex',
					flexDirection: 'column',
					position: 'relative',
					borderRight: '1px solid oklch(0.305 0.010 248)',
					'@media (max-width: 900px)': {
						flex: 'none',
						maxWidth: 'none',
						minWidth: 0,
						width: '100%',
						borderRight: 'none',
						borderBottom: '1px solid oklch(0.305 0.010 248)',
					},
				}}
			>
				<LoginBox />
			</Box>

			{/* Visual pane */}
			<Box
				component="section"
				sx={{
					flex: '1 1 auto',
					position: 'relative',
					overflow: 'hidden',
					bgcolor: 'oklch(0.135 0.006 248)',
					minWidth: 0,
					'@media (max-width: 900px)': {
						minHeight: '320px',
						order: -1,
					},
				}}
			>
				<VisualPane />
			</Box>
		</Box>
	)
}
