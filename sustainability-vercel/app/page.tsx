"use client";
import Image from "next/image";
import { useState } from "react";
import AboutUsModal from "../app/components/AboutUsModal";
import senaPhoto from './images/sena.png';

export default function PlayPage() {
  const [openAboutUs, setOpenAboutUs] = useState(false);

  return (
    <main className="app-container">
      {/* Header */}
      <header className="app-header">
        <Image
            src={senaPhoto}
            alt="Sena Logo"
            width={200}
            height={200}
          />
        <h1>Sena</h1>
        <div className="header-buttons">
          <button onClick={() => setOpenAboutUs(true)}>
            About Us
          </button>
        </div>
      </header>

      {/* Content */}
      <div className="app-content">
        {/* Info Panel */}
        <aside className="info-panel">
          <h2>How to Use</h2>
          <ol>
            <li>
              <strong>Learning Mode: </strong> 
              <br />
              This stage provides guidance, explanations, and context. You can learn about the different 
              pillars of software sustainability, try out simulations to see the effects of your decisions, 
              and ask questions about software sustainability. The goal is to solidify your understanding of sustainability 
              principles and decision-making strategies.
            </li>
            <li>
              <strong>Playing Mode: </strong> 
              <br />
              In this stage, you interact directly with the simulation. You read scenarios, make decisions, 
              and observe the immediate outcomes of your choices in the system. This stage is meant to 
              immerse you in real-world trade-offs and help you explore sustainability challenges firsthand.
            </li>
            <li>
              <strong>Reflection: </strong> 
              <br />
              Finally, you reflect on your decisions and the insights gained from the simulation. 
              You can compare different approaches, consider alternative strategies, and consolidate 
              key lessons. This stage encourages critical thinking and helps you connect the simulation 
              experience to real-world applications.
            </li>
          </ol>
          <p>
            Each stage builds on your knowledge to ensure that your experience is interactive, educational, 
            and reflective. The combination of decision-making, learning feedback, and reflection helps 
            deepen your understanding of complex sustainability systems.
          </p>
        </aside>

        {/* Unity Embed */}
        <section className="unity-panel">
          <iframe
            src="/unity/index.html"
            allow="fullscreen"
            className="unity-iframe"
          />
        </section>

        {/* About Us Modal */}
        <AboutUsModal
          isOpen={openAboutUs}
          onClose={() => setOpenAboutUs(false)}
        />
      </div>
    </main>
  );
}