import { Box, Container} from '@mui/material'
import { useTheme } from '@mui/material/styles'
import { InfoSection, LoginBox } from '../components/login'


export default function Login() {
	const theme = useTheme()
	return (
		<>
            <Box
                sx={{
                    minHeight: "100vh",
                    bgcolor: theme.palette.background.default,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center"
                }}
            >
                <Container maxWidth="lg">
                    <Box
                        sx={{
                            display: "grid",
                            gridTemplateColumns: "1fr 1fr",
                            gap: 8,
                            alignItems: "center",
                            '@media (max-width: 900px)': {
                                gridTemplateColumns: '1fr',
                                gap: 4
                            }
                        }}
                    >
                        <InfoSection />

                        <LoginBox />
                    </Box>
                </Container>
            </Box>
		</>
	)
}
