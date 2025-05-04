import { MantineProvider, useMantineColorScheme, ColorSchemeScript, createTheme } from '@mantine/core';
import { useLocalStorage } from '@mantine/hooks';
import '@mantine/core/styles.css';                    // глобальные стили и CSS-переменные
import './global.css';                                 // ваш CSS для body или data-* атрибутов
import { AppRouter } from './router';

const themeOverrides = createTheme({
    primaryColor: 'blue',
    fontFamily: 'Inter, sans-serif',
});

export function App() {
    const [scheme, setScheme] = useLocalStorage<'light'|'dark'|'auto'>({
        key: 'mantine-color-scheme',
        defaultValue: 'light',
        getInitialValueInEffect: true,
    });

    return (
        <>
            <ColorSchemeScript defaultColorScheme="light" />

            <MantineProvider
                defaultColorScheme={scheme}
                colorSchemeManager={{
                    get:    () => scheme,
                    set:    (val) => setScheme(val),
                    subscribe:    () => {},
                    unsubscribe:  () => {},
                    clear:  () => setScheme('dark'),
                }}
                theme={themeOverrides}
            >
                <AppContent />
            </MantineProvider>
        </>
    );
}

function AppContent() {
    const { colorScheme, setColorScheme, toggleColorScheme, clearColorScheme } = useMantineColorScheme();

    return (
        <>
            <button onClick={() => toggleColorScheme()}>
                {colorScheme === 'dark' ? 'Светлая тема' : 'Тёмная тема'}
            </button>
            <AppRouter />
        </>
    );
}
export default App;