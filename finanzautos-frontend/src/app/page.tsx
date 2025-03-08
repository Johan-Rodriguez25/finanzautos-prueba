"use client";

import { Button } from "@/components/ui/button";
import Link from "next/link";
import { useEffect, useState } from "react";

function HomePage() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    try {
      const token = localStorage.getItem("token");
      setIsLoggedIn(!!token);
    } catch (error) {
      console.error("Error checking authentication:", error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  return (
    <section className="min-h-[calc(100vh-7rem)] flex justify-center items-center p-4">
      <div className="text-center max-w-3xl mx-auto">
        <h1 className="text-slate-200 font-bold text-3xl md:text-5xl mb-4 md:mb-6">
          Finanzautos
        </h1>
        <p className="text-slate-400 mb-6 md:mb-8 text-base md:text-lg">
          Plataforma para gestionar tus publicaciones y perfil de usuario.
        </p>

        {isLoading ? (
          <div className="flex justify-center">
            <p className="text-slate-400">Cargando...</p>
          </div>
        ) : isLoggedIn ? (
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button
              asChild
              className="bg-blue-600 hover:bg-blue-700 w-full sm:w-auto"
            >
              <Link href="/dashboard">Dashboard</Link>
            </Button>
            <Button
              asChild
              variant="outline"
              className="border-white text-white hover:bg-gray-800 w-full sm:w-auto"
            >
              <Link href="/dashboard/profile">Mi Perfil</Link>
            </Button>
          </div>
        ) : (
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button
              asChild
              className="bg-blue-600 hover:bg-blue-700 w-full sm:w-auto"
            >
              <Link href="/auth/login">Iniciar Sesión</Link>
            </Button>
            <Button
              asChild
              variant="outline"
              className="border-white text-white hover:bg-gray-800 w-full sm:w-auto"
            >
              <Link href="/auth/register">Registrarse</Link>
            </Button>
          </div>
        )}
      </div>
    </section>
  );
}

export default HomePage;
