// Servicio de autenticación para manejar tokens y refresh

// Tiempo en milisegundos antes de que expire el token para intentar refrescarlo
// Por ejemplo, 5 minutos antes de que expire
// const REFRESH_THRESHOLD = 5 * 60 * 1000;
const REFRESH_THRESHOLD = 30 * 1000;

// Almacena el temporizador para el refresh automático
let refreshTimer: NodeJS.Timeout | null = null;

// Función para obtener el token actual
export const getToken = (): string | null => {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("token");
};

// Función para guardar el token y configurar el refresh automático
export const setToken = (token: string, expiresIn?: number): void => {
  if (typeof window === "undefined") return;

  // Guardar en localStorage
  localStorage.setItem("token", token);

  // Guardar en cookie
  const maxAge = expiresIn ? Math.floor(expiresIn / 1000) : 60 * 60 * 24 * 7; // 7 días por defecto
  document.cookie = `token=${token}; path=/; max-age=${maxAge}`;

  // Si tenemos información sobre cuándo expira, configuramos el refresh automático
  if (expiresIn) {
    scheduleTokenRefresh(expiresIn);
  }
};

// Función para eliminar el token y cancelar cualquier refresh pendiente
export const removeToken = (): void => {
  if (typeof window === "undefined") return;

  // Eliminar de localStorage
  localStorage.removeItem("token");

  // Eliminar la cookie
  document.cookie = "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";

  // Cancelar cualquier temporizador de refresh pendiente
  if (refreshTimer) {
    clearTimeout(refreshTimer);
    refreshTimer = null;
  }
};

// Función para programar el refresh automático del token
const scheduleTokenRefresh = (expiresIn: number): void => {
  // Cancelar cualquier temporizador existente
  if (refreshTimer) {
    clearTimeout(refreshTimer);
  }

  // Calcular cuándo debemos refrescar el token (antes de que expire)
  const refreshTime = expiresIn - REFRESH_THRESHOLD;

  // Solo programar si el tiempo es positivo
  if (refreshTime > 0) {
    refreshTimer = setTimeout(async () => {
      await refreshToken();
    }, refreshTime);
  }
};

// Función para refrescar el token
export const refreshToken = async (): Promise<boolean> => {
  try {
    const currentToken = getToken();

    if (!currentToken) {
      return false;
    }

    const res = await fetch(`http://localhost:8080/api/User/refresh`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${currentToken}`,
      },
    });

    if (!res.ok) {
      // Si el refresh falla, podríamos redirigir al login
      removeToken();
      return false;
    }

    const data = await res.json();

    if (data.token) {
      // Guardar el nuevo token con 20 minutos de expiración
      setToken(data.token, 30 * 1000); // 20 minutos en milisegundos
      return true;
    }

    return false;
  } catch (error) {
    console.error("Error refreshing token:", error);
    return false;
  }
};

// Función para verificar si el token está por expirar y refrescarlo si es necesario
export const checkAndRefreshToken = async (): Promise<boolean> => {
  const token = getToken();

  if (!token) {
    return false;
  }

  try {
    // Intentar decodificar el token para ver cuándo expira
    // Nota: Esto asume que el token es JWT. Si no lo es, necesitarás otra lógica.
    const tokenParts = token.split(".");
    if (tokenParts.length !== 3) {
      return false;
    }

    const payload = JSON.parse(atob(tokenParts[1]));
    const expiryTime = payload.exp * 1000; // Convertir a milisegundos
    const currentTime = Date.now();

    // Si el token expira pronto, refrescarlo
    if (expiryTime - currentTime < REFRESH_THRESHOLD) {
      return await refreshToken();
    }

    return true;
  } catch (error) {
    console.error("Error checking token expiry:", error);
    return false;
  }
};
