"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { UserCircle, Menu, X } from "lucide-react";

function Navbar() {
  const router = useRouter();
  const pathname = usePathname();
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  useEffect(() => {
    // Verificar si el usuario está autenticado
    const token = localStorage.getItem("token");
    setIsLoggedIn(!!token);
  }, [pathname]); // Re-verificar cuando cambia la ruta

  const handleLogout = async () => {
    try {
      const token = localStorage.getItem("token");

      if (!token) {
        router.push("/auth/login");
        return;
      }

      // Llamar al endpoint de logout
      const res = await fetch(`http://localhost:8080/api/User/logout`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) {
        if (res.status === 401) {
          // Token inválido
          localStorage.removeItem("token");
          document.cookie =
            "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
          router.push("/auth/login");
          return;
        }
      }

      // Limpiar token y cookie
      localStorage.removeItem("token");
      document.cookie = "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";

      // Redirigir al login
      router.push("/auth/login");
      router.refresh();
    } catch (error) {
      console.error("Logout error:", error);
      localStorage.removeItem("token");
      document.cookie = "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
      router.push("/auth/login");
    }
  };

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  const closeMenu = () => {
    setIsMenuOpen(false);
  };

  return (
    <nav className="border-b border-gray-800 bg-gray-950 py-4">
      <div className="container mx-auto flex items-center justify-between px-4">
        <Link href="/" className="text-xl font-bold text-white">
          Finanzautos
        </Link>

        {/* Botón de menú móvil */}
        <button
          className="md:hidden text-white p-2"
          onClick={toggleMenu}
          aria-label="Toggle menu"
        >
          {isMenuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>

        {/* Menú de navegación */}
        <ul
          className={`
          flex flex-col md:flex-row items-center gap-4
          ${
            isMenuOpen
              ? "absolute top-16 left-0 right-0 bg-gray-900 p-4 z-50 border-b border-gray-800 shadow-lg"
              : "hidden md:flex"
          }
        `}
        >
          {!isLoggedIn ? (
            <>
              <li className="w-full md:w-auto">
                <Button
                  variant="link"
                  asChild
                  className="w-full md:w-auto justify-center md:justify-start"
                >
                  <Link
                    href="/"
                    className="text-white hover:text-gray-300"
                    onClick={closeMenu}
                  >
                    Home
                  </Link>
                </Button>
              </li>
              <li className="w-full md:w-auto">
                <Button
                  variant="link"
                  asChild
                  className="w-full md:w-auto justify-center md:justify-start"
                >
                  <Link
                    href="/auth/login"
                    className="text-white hover:text-gray-300"
                    onClick={closeMenu}
                  >
                    Login
                  </Link>
                </Button>
              </li>
              <li className="w-full md:w-auto">
                <Button
                  variant="outline"
                  asChild
                  className="text-white border-white hover:bg-gray-800 w-full md:w-auto"
                >
                  <Link href="/auth/register" onClick={closeMenu}>
                    Register
                  </Link>
                </Button>
              </li>
            </>
          ) : (
            <>
              <li className="w-full md:w-auto">
                <Button
                  variant="link"
                  asChild
                  className="w-full md:w-auto justify-center md:justify-start"
                >
                  <Link
                    href="/dashboard"
                    className="text-white hover:text-gray-300"
                    onClick={closeMenu}
                  >
                    Dashboard
                  </Link>
                </Button>
              </li>
              <li className="w-full md:w-auto">
                <Button
                  variant="link"
                  asChild
                  className="w-full md:w-auto justify-center md:justify-start"
                >
                  <Link
                    href="/dashboard/profile"
                    className="text-white hover:text-gray-300 flex items-center gap-2"
                    onClick={closeMenu}
                  >
                    <UserCircle size={18} />
                    Mi Perfil
                  </Link>
                </Button>
              </li>
              <li className="w-full md:w-auto">
                <Button
                  variant="destructive"
                  onClick={() => {
                    closeMenu();
                    handleLogout();
                  }}
                  className="bg-red-600 hover:bg-red-700 w-full md:w-auto"
                >
                  Cerrar sesión
                </Button>
              </li>
            </>
          )}
        </ul>
      </div>
    </nav>
  );
}

export default Navbar;
