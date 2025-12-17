"use client";

import { useState } from "react";
import AboutUsModal from "../app/components/AboutUsModal";

export default function PlayPage() {
  const [activeTab, setActiveTab] = useState<"about" | "how">("about");
  const [openAboutUs, setOpenAboutUs] = useState(false);

  return (
    <main className="app-container">
      {/* Header */}
      <header className="app-header">
        <h1>Sustainability Software Game</h1>
      </header>

      {/* Content */}
      <div className="app-content">
        {/* Info Panel */}
        <aside className="info-panel">
          <div className="header-buttons">
            <button onClick={() => setActiveTab("about")}>About</button>
            <button onClick={() => setActiveTab("how")}>How to Use</button>
            <button
              onClick={() => setOpenAboutUs(true)}
              className="modal-open-button"
            >
              About Us
            </button>
          </div>
          
          {activeTab === "about" && (
            <>
              <h2>About</h2>
              <p>
                ___ is an interactive simulation tool designed 
                to explore complex trade-offs in decision-making related to sustainability. 
                Users navigate scenarios that involve environmental, economic, and social factors, 
                making choices that influence outcomes across multiple systems. The tool aims 
                to foster systems thinking, critical analysis, and awareness of long-term 
                consequences in policy, industry, and personal decision-making.  
                It’s ideal for students, researchers, and practitioners interested in sustainability, 
                software-assisted learning, and decision science.
              </p>
            </>
          )}
          {activeTab === "how" && (
            <>
              <h2>How to Use</h2>
              <ol>
                <li>Read the scenario.</li>
                <li>Make decisions as prompted.</li>
                <li>Observe the outcomes.</li>
              </ol>
            </>
          )}
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