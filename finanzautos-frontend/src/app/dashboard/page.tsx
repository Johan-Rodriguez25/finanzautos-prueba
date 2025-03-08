"use client";
import { Button } from "@/components/ui/button";
import { useRouter } from "next/navigation";
import { useState, useEffect, useCallback } from "react";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { PlusCircle, Trash2 } from "lucide-react";
import { refreshToken, checkAndRefreshToken } from "@/lib/auth";

interface Post {
  id: string;
  title: string;
  content: string;
  userId: string;
  createdAt: string;
  updatedAt: string;
}

function DashboardPage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  const [isCreating, setIsCreating] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [newPost, setNewPost] = useState({
    title: "",
    content: "",
  });

  useEffect(() => {
    const verifyToken = async () => {
      const isValid = await checkAndRefreshToken();
      if (!isValid) {
        router.push("/auth/login");
      }
    };

    verifyToken();

    const interval = setInterval(verifyToken, 30 * 1000);

    return () => clearInterval(interval);
  }, [router]);

  const fetchPosts = useCallback(async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("token");

      if (!token) {
        router.push("/auth/login");
        return;
      }

      const res = await fetch(
        `${process.env.NEXT_PUBLIC_PUBLICATION_MICROSERVICE_URL}/user`,
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
          const refreshed = await refreshToken();

          if (refreshed) {
            return fetchPosts();
          }

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
      setPosts(data);
    } catch (err) {
      console.error("Error fetching posts:", err);
      setError("Error al cargar las publicaciones");
    } finally {
      setLoading(false);
    }
  }, [router]);

  useEffect(() => {
    fetchPosts();
  }, [fetchPosts]);

  const createPost = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    try {
      const token = localStorage.getItem("token");

      if (!token) {
        router.push("/auth/login");
        return;
      }

      if (!newPost.title.trim() || !newPost.content.trim()) {
        setError("El título y el contenido son obligatorios");
        return;
      }

      const res = await fetch(
        `${process.env.NEXT_PUBLIC_PUBLICATION_MICROSERVICE_URL}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            title: newPost.title,
            content: newPost.content,
          }),
        }
      );

      if (!res.ok) {
        if (res.status === 401) {
          const refreshed = await refreshToken();

          if (refreshed) {
            return createPost(e);
          }

          setError("Sesión expirada. Por favor inicie sesión nuevamente.");
          localStorage.removeItem("token");
          document.cookie =
            "token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
          router.push("/auth/login");
          return;
        }
        throw new Error(`Error: ${res.status}`);
      }

      setNewPost({ title: "", content: "" });
      setSuccess("Publicación creada exitosamente");
      setIsCreating(false);

      fetchPosts();
    } catch (err) {
      console.error("Error creating post:", err);
      setError("Error al crear la publicación");
    }
  };

  const deletePost = async (id: string) => {
    setError(null);
    setSuccess(null);
    setDeletingId(id);

    try {
      const token = localStorage.getItem("token");

      if (!token) {
        router.push("/auth/login");
        return;
      }

      const res = await fetch(
        `${process.env.NEXT_PUBLIC_PUBLICATION_MICROSERVICE_URL}/${id}`,
        {
          method: "DELETE",
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

      setSuccess("Publicación eliminada exitosamente");

      setPosts(posts.filter((post) => post.id !== id));
    } catch (err) {
      console.error("Error deleting post:", err);
      setError("Error al eliminar la publicación");
    } finally {
      setDeletingId(null);
    }
  };

  useEffect(() => {
    fetchPosts();
  }, []);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("es-ES", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  return (
    <section className="min-h-[calc(100vh-7rem)] py-6 md:py-10 px-4">
      <div className="container mx-auto">
        <div className="flex justify-between items-center mb-6 md:mb-8">
          <h1 className="text-slate-200 font-bold text-2xl md:text-4xl">
            Dashboard
          </h1>
        </div>

        {error && (
          <div className="bg-red-500 text-white p-3 rounded mb-4 md:mb-6 text-sm">
            {error}
          </div>
        )}

        {success && (
          <div className="bg-green-500 text-white p-3 rounded mb-4 md:mb-6 text-sm">
            {success}
          </div>
        )}

        <div className="bg-gray-900 rounded-lg p-4 md:p-6 shadow-lg mb-6 md:mb-8">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-4 md:mb-6 gap-3">
            <h2 className="text-slate-200 text-xl md:text-2xl font-semibold">
              {isCreating ? "Nueva Publicación" : "Crear Publicación"}
            </h2>
            {!isCreating && (
              <Button
                onClick={() => setIsCreating(true)}
                className="bg-blue-600 hover:bg-blue-700 flex items-center gap-2 w-full sm:w-auto"
              >
                <PlusCircle size={18} />
                Crear nueva
              </Button>
            )}
          </div>

          {isCreating && (
            <form onSubmit={createPost} className="space-y-4">
              <div>
                <label htmlFor="title" className="block text-slate-300 mb-2">
                  Título
                </label>
                <Input
                  id="title"
                  value={newPost.title}
                  onChange={(e) =>
                    setNewPost({ ...newPost, title: e.target.value })
                  }
                  placeholder="Ingrese el título de la publicación"
                  className="bg-gray-800 border-gray-700 text-white"
                />
              </div>

              <div>
                <label htmlFor="content" className="block text-slate-300 mb-2">
                  Contenido
                </label>
                <Textarea
                  id="content"
                  value={newPost.content}
                  onChange={(e) =>
                    setNewPost({ ...newPost, content: e.target.value })
                  }
                  placeholder="Escriba el contenido de su publicación"
                  className="bg-gray-800 border-gray-700 text-white min-h-[120px]"
                />
              </div>

              <div className="flex flex-col sm:flex-row justify-end gap-3">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setIsCreating(false);
                    setNewPost({ title: "", content: "" });
                  }}
                  className="border-gray-600 text-gray-300 hover:bg-gray-800 w-full sm:w-auto"
                >
                  Cancelar
                </Button>
                <Button
                  type="submit"
                  className="bg-blue-600 hover:bg-blue-700 w-full sm:w-auto"
                >
                  Publicar
                </Button>
              </div>
            </form>
          )}
        </div>

        <div className="bg-gray-900 rounded-lg p-4 md:p-6 shadow-lg">
          <h2 className="text-slate-200 text-xl md:text-2xl font-semibold mb-4 md:mb-6">
            Mis Publicaciones
          </h2>

          {loading ? (
            <div className="text-center py-8 md:py-10">
              <p className="text-slate-400">Cargando publicaciones...</p>
            </div>
          ) : posts.length === 0 ? (
            <div className="text-center py-8 md:py-10 border border-dashed border-gray-700 rounded-lg">
              <p className="text-slate-400">No hay publicaciones disponibles</p>
            </div>
          ) : (
            <div className="grid gap-4 md:gap-6">
              {posts.map((post) => (
                <div
                  key={post.id}
                  className="bg-gray-800 rounded-lg p-4 md:p-5 hover:bg-gray-750 transition-colors"
                >
                  <div className="flex justify-between items-start mb-2">
                    <h3 className="text-slate-200 text-lg md:text-xl font-medium pr-2">
                      {post.title}
                    </h3>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => deletePost(post.id)}
                      disabled={deletingId === post.id}
                      className="text-red-400 hover:text-red-300 hover:bg-gray-700 p-2 h-auto flex-shrink-0"
                    >
                      {deletingId === post.id ? (
                        <span className="text-xs">Eliminando...</span>
                      ) : (
                        <Trash2 size={18} />
                      )}
                    </Button>
                  </div>
                  <p className="text-slate-400 mb-4 break-words">
                    {post.content}
                  </p>
                  <div className="text-right text-xs md:text-sm text-slate-500">
                    <span>Creado: {formatDate(post.createdAt)}</span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </section>
  );
}

export default DashboardPage;
