import Image from "next/image";
import pluriseLogo from '../images/plurise.png';
import keerynPhoto from '../images/KeerynJohnson.jpg';
import ronniePhoto from '../images/SouzaSantos.jpg';

interface AboutUsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function AboutUsModal({ isOpen, onClose }: AboutUsModalProps) {
  if (!isOpen) return null;

  return (
    <div className="about-modal-overlay" onClick={onClose}>
      <div className="about-modal" onClick={(e) => e.stopPropagation()}>
        <h2>About Us</h2>

        <div className="about-section">
          <Image
            src={pluriseLogo}
            alt="Plurise Logo"
            width={200}
            height={200}
            className="about-img"
          />
          <div>
            <h4>Plurise Lab</h4>
            <p>
              The Plural Software Engineering for a Plural Society lab advances a socio technical understanding 
              of software engineering with emphasis on software processes and teamwork. 
              The lab conducts empirical studies on how human behavior, organizational context, and development 
              practices shape software systems, alongside research on software fairness, testing, and AI enabled 
              technologies. As software increasingly mediates work, education, and everyday life, the lab’s 
              research contributes evidence and conceptual frameworks that support software systems designed to 
              account for the plurality of society.
            </p>
          </div>
        </div>

        {/* Team Members */}
        <div className="about-section">
          <Image
            src={keerynPhoto}
            alt="Keeryn Johnson Portrait"
            width={200}
            height={200}
            className="about-img"
          />
          <div>
            <h4>Keeryn Johnson (Undergraduate Research Assistant)</h4>
            <p>
              Keeryn Johnson is pursuing a Software Engineering degree with a minor in Mechatronics 
              at the University of Calgary. He is interested in robotics, interactive software design, 
              and the development of educational simulations that integrate real-world decision-making 
              and systems thinking.
            </p>
          </div>
        </div>

        <div className="about-section">
          <Image
            src={ronniePhoto}
            alt="Dr. Ronnie De Souza Santos Portrait"
            width={200}
            height={200}
            className="about-img"
          />
          <div>
            <h4>Dr. Ronnie de Souza Santos, Ph.D.</h4>
            <p>
              Dr. Ronnie de Souza Santos is an Assistant Professor in Software Engineering at the University of Calgary. 
              His research focuses on the human aspects of software engineering, software project management, 
              software quality and testing, fairness in software, and inclusive design practices. 
              Dr. Santos supports the development of tools that combine rigorous technical modeling 
              with human-centered decision-making.
            </p>
          </div>
        </div>

        <div className="about-modal-actions">
          <button className="close-button" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}