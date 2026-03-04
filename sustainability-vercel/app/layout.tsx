import './globals.css';

export const metadata = {
  title: "Sena",
  description:
    "Sena is an interactive learning and simulation platform for software sustainability.",
  icons: {
    icon: "/sena.png",
  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}