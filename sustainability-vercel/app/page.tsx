"use client";
import Image from "next/image";
import { useState } from "react";
import AboutUsModal from "../app/components/AboutUsModal";
import senaPhoto from './images/sena.png';

export default function PlayPage() {
  const [openAboutUs, setOpenAboutUs] = useState(false);
  const [activeTab, setActiveTab] = useState("Intro");

  const renderTabContent = () => {
    switch (activeTab) {
      case "Intro":
        return (
          <>
            <strong>Welcome to Sena! </strong>
            <p>
              Sena is an interactive learning and simulation platform designed to explore the principles of software sustainability. 
              It helps users understand how decisions in software development impact not only the technical quality of a system but 
              also its environmental, social, and long-term economic sustainability.
              <br /><br />
              Sena is intended for students, educators, and professionals in software engineering, computer science, and related fields
              who want to deepen their understanding of sustainable development practices. Through guided learning, hands-on simulations,
              and reflective exercises, Sena allows users to experiment with real-world scenarios, see the consequences of their decisions,
              and develop the skills needed to create software that is efficient, responsible, and future-ready.
              <br /><br />
              Whether you are learning the fundamentals of sustainable software design or exploring complex trade-offs in advanced projects, 
              Sena provides a safe, interactive environment to test ideas, gain insights, and build sustainable thinking into your 
              development practice.
            </p>
          </>
        );
      case "Learning":
        return (
          <>
            <ol>
              <li>
                <strong>Learning Mode: </strong>
                <br />
                This stage provides guidance, explanations, and context. You can learn about the different 
                pillars of software sustainability, try out simulations to see the effects of your decisions, 
                and ask questions about software sustainability. The goal is to solidify your understanding of sustainability 
                principles and decision-making strategies. The stage features extra info through Hear More's, which give more 
                information about the slide topic, and simulations to reinforce your learning.
              </li>
            </ol>
          </>
        );
      case "Practicing":
        return (
          <ol>
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
              After the Playing Mode, you reflect on your decisions and the insights gained from the simulation. 
              You can compare different approaches, consider alternative strategies, and consolidate 
              key lessons. This stage encourages critical thinking and helps you connect the simulation 
              experience to real-world applications.
            </li>
          </ol>
        );
        
      case "Applying":
        return (
          <ol>
            <li>
              <strong>Quiz Mode: </strong>
              <br />
              This stage is designed to test your knowledge in a multiple choice, quiz like scenario. You are given four options to
              choose from based on the scenario, and must choose the best response. The chat feature is also available in this stage,
              allowing you to ask any questions you may have regarding sustainability topics. 
            </li>
            <li>
              <strong>Challenge Mode: </strong>
              <br />
              This stage is designed to test your knowledge in a real life situation, where you interact with your boss at a software company.
              You are tasked with assisting your boss with various different scenarios, and must choose the best developer and strategy for the situation.
              Based on your choices, you see live feedback through the developer and player health bars, as well as feedback from your boss to evaluate
              your learning.
            </li>
          </ol>
        );
      default:
        return null;
    }
  };

  return (
    <main className="app-container">
      {/* Header */}
      <header className="app-header">
        <Image
            src={senaPhoto}
            alt="Sena Logo"
            width={80}
            height={80}
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
          <div className="tabs">
            <button
              className={activeTab === "Intro" ? "active" : ""}
              onClick={() => setActiveTab("Intro")}
            >
              Intro
            </button>
            <button
              className={activeTab === "Learning" ? "active" : ""}
              onClick={() => setActiveTab("Learning")}
            >
              Learning
            </button>
            <button
              className={activeTab === "Practicing" ? "active" : ""}
              onClick={() => setActiveTab("Practicing")}
            >
              Practicing
            </button>
            <button
              className={activeTab === "Applying" ? "active" : ""}
              onClick={() => setActiveTab("Applying")}
            >
              Applying
            </button>
          </div>

          <div className="tab-content">
            {renderTabContent()}
          </div>
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