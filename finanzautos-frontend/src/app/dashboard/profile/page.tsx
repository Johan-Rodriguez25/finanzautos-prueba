"use client";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useState, useEffect } from "react";

const formSchema = z.object({
  name: z.string().min(3, {
    message: "El nombre debe tener al menos 3 caracteres",
  }),
  email: z.string().email({
    message: "Por favor ingrese un correo electrónico válido",
  }),
  password: z
    .string()
    .min(8, {
      message: "La contraseña debe tener al menos 8 caracteres",
    })
    .optional()
    .or(z.literal("")),
});

interface UserUpdateData {
  name: string;
  email: string;
  password?: string;
}

function ProfilePage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      email: "",
      password: "",
    },
  });

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        setLoading(true);
        const token = localStorage.getItem("token");

        if (!token) {
          router.push("/auth/login");
          return;
        }

        const res = await fetch(
          `${process.env.NEXT_PUBLIC_USER_MICROSERVICE_URL}/me`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!res.ok) {
          if (res.status === 401) {
            setError("Sesión expirada. Por favor inicie sesión nuevamente.");
            localStorage.removeItem("token");
            document.cookie =
              "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
            router.push("/auth/login");
            return;
          }
          throw new Error(`Error: ${res.status}`);
        }

        const data = await res.json();

        form.reset({
          name: data.name,
          email: data.email,
          password: "",
        });
      } catch (err) {
        console.error("Error fetching user data:", err);
        setError("Error al cargar los datos del usuario");
      } finally {
        setLoading(false);
      }
    };

    fetchUserData();
  }, [router, form]);

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      setError(null);
      setSuccess(null);

      const token = localStorage.getItem("token");

      if (!token) {
        router.push("/auth/login");
        return;
      }

      const updateData: UserUpdateData = {
        name: values.name,
        email: values.email,
      };

      if (values.password && values.password.trim() !== "") {
        updateData.password = values.password;
      }

      const res = await fetch(
        `${process.env.NEXT_PUBLIC_USER_MICROSERVICE_URL}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(updateData),
        }
      );

      if (!res.ok) {
        if (res.status === 401) {
          setError("Sesión expirada. Por favor inicie sesión nuevamente.");
          localStorage.removeItem("token");
          document.cookie =
            "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
          router.push("/auth/login");
          return;
        }

        try {
          const errorData = await res.json();
          setError(errorData.message || `Error: ${res.status}`);
        } catch {
          setError(`Error al actualizar el perfil: ${res.status}`);
        }
        return;
      }

      setSuccess("Perfil actualizado exitosamente");

      if (values.password) {
        form.setValue("password", "");
      }

      setTimeout(() => {
        router.push("/dashboard");
      }, 1500);
    } catch (err) {
      console.error("Error updating profile:", err);
      setError("Error al actualizar el perfil");
    }
  };

  return (
    <section className="min-h-[calc(100vh-7rem)] py-10 px-4">
      <div className="container mx-auto max-w-2xl">
        <div className="mb-8">
          <Button
            variant="outline"
            onClick={() => router.push("/dashboard")}
            className="mb-4"
          >
            ← Volver al Dashboard
          </Button>
          <h1 className="text-slate-200 font-bold text-4xl">Mi Perfil</h1>
        </div>

        {error && (
          <div className="bg-red-500 text-white p-3 rounded mb-6 text-sm">
            {error}
          </div>
        )}

        {success && (
          <div className="bg-green-500 text-white p-3 rounded mb-6 text-sm">
            {success}
          </div>
        )}

        <div className="bg-gray-900 rounded-lg p-6 shadow-lg">
          {loading ? (
            <div className="text-center py-10">
              <p className="text-slate-400">Cargando datos del usuario...</p>
            </div>
          ) : (
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-6"
              >
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-slate-300">Nombre</FormLabel>
                      <FormControl>
                        <Input
                          {...field}
                          placeholder="Tu nombre"
                          className="bg-gray-800 border-gray-700 text-white"
                        />
                      </FormControl>
                      <FormMessage className="text-red-400" />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="email"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-slate-300">
                        Correo electrónico
                      </FormLabel>
                      <FormControl>
                        <Input
                          {...field}
                          type="email"
                          placeholder="tu@email.com"
                          className="bg-gray-800 border-gray-700 text-white"
                        />
                      </FormControl>
                      <FormMessage className="text-red-400" />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-slate-300">
                        Nueva contraseña (opcional)
                      </FormLabel>
                      <FormControl>
                        <Input
                          {...field}
                          type="password"
                          placeholder="Dejar en blanco para mantener la actual"
                          className="bg-gray-800 border-gray-700 text-white"
                        />
                      </FormControl>
                      <FormMessage className="text-red-400" />
                    </FormItem>
                  )}
                />

                <Button
                  type="submit"
                  className="bg-blue-600 hover:bg-blue-700 w-full"
                >
                  Actualizar perfil
                </Button>
              </form>
            </Form>
          )}
        </div>
      </div>
    </section>
  );
}

export default ProfilePage;
