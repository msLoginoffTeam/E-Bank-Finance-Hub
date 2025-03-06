import { MantineProvider } from '@mantine/core';
import { AppRouter } from './router';
import '@mantine/core/styles.css';

// const theme = createTheme({
//     primaryColor: 'blue', // Цвет по умолчанию для кнопок и др.
//     fontFamily: 'Inter, sans-serif',
// });

function App() {
    return (
        <MantineProvider>
            <AppRouter />
        </MantineProvider>
    );
}

export default App;